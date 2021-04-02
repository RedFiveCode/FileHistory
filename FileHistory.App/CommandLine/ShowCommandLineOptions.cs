using CommandLine;

namespace FileHistory.App
{
    [Verb("show", isDefault:true, HelpText = "Show file history files")]
    public class ShowCommandLineOptions : CommandLineOptions
    {
        [Option('h', "hide", Required = false, HelpText = "Hide display of files with only one generation (and no history).")]
        public bool Hide { get; set; }
    }
}
