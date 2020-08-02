using System;
using System.IO;

namespace ChangeDetector.Core
{
    public static class StringFileExtensions
    {
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            var normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                .WithEnding("\\"));

            var normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        public static string WithEnding( this string str, string ending)
        {
            if (str == null)
                return ending;

            var result = str;

            for (var i = 0; i <= ending.Length; i++)
            {
                var tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

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