using CommandLine;

namespace FileHistory.App
{
    [Verb("show", isDefault:true, HelpText = "Show file history files")]
    public class ShowCommandLineOptions : CommandLineOptions
    {
    }
}
