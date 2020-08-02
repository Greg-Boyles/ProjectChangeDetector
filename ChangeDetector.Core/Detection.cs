using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChangeDetector.Core.Processor;

namespace ChangeDetector.Core
{
    public class Detection : IDetection
    {
        private ChangeLog _changeLog;
        private ICsprojProcessor _csprojProcessor;
        private List<string> _projectFolders;

        public static IDetection Build() =>
            new Detection();

        public static IDetection Build(ChangeLog changeLog) =>
            new Detection().LoadInChangeLog(changeLog);

        public IDetection LoadInChangeLog(ChangeLog changeLog)
        {
            _changeLog = changeLog;
            return this;
        }

        public IDetection Run()
        {
            _csprojProcessor = CsprojProcessorFactory.CreateInstance();

            var projectReferences = RecursiveSubProjectCheck(_changeLog.CsprojPath);
            projectReferences.Add(_changeLog.CsprojPath);

            _projectFolders = projectReferences.ConvertAll(Path.GetDirectoryName).ToList();

            foreach (var projectFolder in _projectFolders)
            {
                var changeFound = ProjectContainsChangedFiles(projectFolder);

                if (changeFound)
                {
                    Console.WriteLine($"File change found in project Project: {projectFolder}");
                }
            }

            return this;
        }

        private List<string> RecursiveSubProjectCheck(string csprojPath)
        {
            var projectReferences = _csprojProcessor.GetProjectReference(csprojPath);

            if (projectReferences.Count == 0) return projectReferences;

            var subProjectReferences = new List<string>();
            foreach (var subProject in projectReferences)
            {
                subProjectReferences.AddRange(RecursiveSubProjectCheck(subProject));
            }

            projectReferences.AddRange(subProjectReferences);

            return projectReferences.Distinct().ToList();
        }

        private bool ProjectContainsChangedFiles(string projectFolder)
        {
            foreach (var changedFile in _changeLog.ChangedFiles.Select(x => x.FilePath))
            {
                var found = changedFile.IsSubPathOf(projectFolder);

                if (found) return true;

            }

            return false;
        }
    }


    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                .WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding( this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }
    }
}
