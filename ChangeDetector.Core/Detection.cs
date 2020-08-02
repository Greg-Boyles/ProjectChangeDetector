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
}
