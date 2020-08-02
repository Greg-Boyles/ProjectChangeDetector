using CommandLine;

namespace ChangeDetector.Console.TeamCity
{
    class CommandLineOptions
    {
        [Option("csorojPath", Required = true, HelpText = "Full file path to the csoroj to run the change detector on")]
        public string CsorojPath { get; set; }

        [Option("changedFilesLog", Required = true, HelpText = "Teamcity changed files text file")]
        public string ChangedFilesLog { get; set; }

        [Option("checkOutPath", Required = true, HelpText = "Teamcity changed files text file")]
        public string CheckOutPath { get; set; }
    }
}
