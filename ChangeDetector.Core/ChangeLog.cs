using System.Collections.Generic;

namespace ChangeDetector.Core
{
    public class ChangeLog
    {
        public ChangeLog()
        {
            ChangedFiles = new List<ChangedFile>();
        }

        public List<ChangedFile> ChangedFiles { get; set; }
        public string CsprojPath { get; set; }

        public class ChangedFile
        {
            public string FilePath { get; set; }
            public string CommitHash { get; set; }
        }
    }
}