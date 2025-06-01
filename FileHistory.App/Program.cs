using CommandLine;
using FileHistory.Utils;
using System;

namespace FileHistory.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<ShowCommandLineOptions, TidyCommandLineOptions>(args)
                              .WithParsed<ShowCommandLineOptions>(cl => Show(cl))
                              .WithParsed<TidyCommandLineOptions>(cl => Tidy(cl));
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine($"Error {ex.Message} at\n{ex.StackTrace}", ConsoleColor.Red);
            }
        }

        static void Show(ShowCommandLineOptions options)
        {
            var operation = new ShowOperation();
            operation.ShowFolder(options);
        }

        static void Tidy(TidyCommandLineOptions options)
        {
            var operation = new TidyOperation();
            operation.TidyFolder(options);
        }
    }
}
