using System.IO;
using ChangeDetector.Core;
using CommandLine;
using Serilog;

namespace ChangeDetector.Console.TeamCity
{
    class Program
    {
        private static int Main(string[] args)
        {
            Log.Debug("Loading command line options");
            CommandLineOptions options = null;
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(op =>
            {
                options = op;
            });

            if (options == null)
            {
                Log.Fatal("Command line options invalid");
                return 1;
            }

            if (!ChangeLogFound(options.ChangedFilesLog))
            {
                Log.Fatal("Change log not found");
                return 1;
            }

            if (!ChangeLogFound(options.CsorojPath))
            {
                Log.Fatal("Csproj not found");
                return 1;
            }

            var changeLog = ProcessChangeLog(options.ChangedFilesLog);
            changeLog.CsprojPath = options.CsorojPath;

            Detection.Build(changeLog).Run();

            return 0;
        }

        private static bool ChangeLogFound(string changeLogFilePath) =>
            File.Exists(changeLogFilePath);

        private static ChangeLog ProcessChangeLog(string changeLogFilePath)
        {
            var result = new ChangeLog();
            string[] rawLogLines = File.ReadAllLines(changeLogFilePath);

            if (rawLogLines.Length == 0)
                return result;

            foreach (var rawLogLine in rawLogLines)
            {
                var split = rawLogLine.Split(':');
                result.ChangedFiles.Add(new ChangeLog.ChangedFile
                {
                    FilePath = split[0],
                    CommitHash = split[2]
                });
            }

            return result;
        }
    }
}
