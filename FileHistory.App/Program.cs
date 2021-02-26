using CommandLine;
using FileHistory.Utils;
using System;
using System.Collections.Generic;

namespace FileHistory.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
            //                        .WithParsed(RunOptions)
            //                        .WithNotParsed(HandleParseError);

            try
            {
                CommandLine.Parser.Default.ParseArguments<ShowCommandLineOptions, TidyCommandLineOptions>(args)
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

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }

    //[Verb("show", HelpText = "Show file history for folder")]
    //public class ShowOptions
    //{
    //    //normal options here
    //}

}
