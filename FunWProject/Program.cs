using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace FunWProject
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messeges.")]
            public bool Verbose { get; set; }

            [Option('n', "namddde", Required = true, HelpText = "Set name")]
            public string name { get; set; }

        }
        [Verb("theme", HelpText = "set theme or vew theme")]
        public class theme
        {
            [Option('c', "currentName", HelpText = "view current name")]
            public bool currentName { get; set; }

            [Option('s', "setname", HelpText = "set name for user")]
            public string setname { get; set; }
        }




        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options,theme>(args)
                .WithParsed<theme>(o =>
                {
                    if (o.currentName)
                        Console.WriteLine("james");

                    Console.WriteLine(o.setname);
                })
                .WithParsed<Options>(o =>
                {
                    if (o.Verbose)
                    {
                        Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                        Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                    }
                    else
                    {
                        Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                        Console.WriteLine("Quick Start Example!");
                    }
                })
                .WithParsed<Options>(o =>
                {
                    Console.WriteLine($"Current Name :{o.name}");
                    Console.WriteLine($"Current val :{o.Verbose}");
                });

        }
    }
}
    //class Options
    //{
    //    [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
    //    public IEnumerable<string> InputFiles { get; set; }

    //    // Omitting long name, defaults to name of property, ie "--verbose"
    //    [Option(
    //        Default = false,
    //        HelpText = "Prints all messeges to standard output.")]
    //    public bool Verbose { get; set; }

    //    [Option("stdin",
    //        Default = false,
    //        HelpText = "Read from stdin")]
    //    public bool stdin { get; set; }

    //    [Value(0, MetaName = "offset", HelpText = "File offset.")]
    //    public long? Offset { get; set; }
    //}

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //            CommandLine.Parser.Default.ParseArguments<Options>(args)
    //            .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
    //            .WithNotParsed<Options>((errs) => HandleParseError(errs));
    //    }
    //}

