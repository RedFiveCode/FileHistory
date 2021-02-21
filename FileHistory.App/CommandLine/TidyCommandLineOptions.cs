using CommandLine;

namespace FileHistory.App
{
    [Verb("tidy", HelpText = "Tidy old file history files")]
    public class TidyCommandLineOptions : CommandLineOptions
    {
        [Option('p', "preview", Required = false, HelpText = "Preview, do not delete files.")]
        public bool Preview { get; set; }

        [Option('k', "keepRecentGenerations", Required = false, HelpText = "Keep the most recent N generations", Default = 1)]
        public int KeepGenerations { get; set; }
    }
}
