using CommandLine;

namespace FileHistory.App
{
 
    /// <summary>
    /// Command line options common to all commands (verbs)
    /// </summary>
    public class CommandLineOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Input folder.")]
        public string Folder { get; set; }

        [Option('r', "recurse", Required = false, HelpText = "Recurse sub-folders.")]
        public bool RecurseSubFolders { get; set; }

        [Option('m', "match", Required = false, HelpText = "Filter using filename wildcard (case insensitive).")]
        public string WildcardFilter { get; set; }

        [Option('g', "minimumSize", Required = false, HelpText = "Filter to include files greater than minimum size.")]
        public long MinimumSize { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
