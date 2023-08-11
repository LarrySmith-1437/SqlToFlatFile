using CommandLine;
using System;

namespace SqlToFlatFile
{
    public class Options
    {
        [Value(0)]
        [Option('c', longName: "ConnectionString", HelpText = "connection string for sql server connection", Required = true)]
        public string ConnectionString { get; set; }

        [Value(1)]
        [Option('o', longName: "OutputFilePath", HelpText = "Path to write the output file to.  If file exists, it will be overwritten.", Required = true)]
        public string OutputFilePath { get; set; }

        [Value(2)]
        [Option('h', longName: "Header", HelpText = "Write column names as headers", Required = false, Default = false)]
        public bool Header { get; set; }

        [Value(3)]
        [Option('s', longName: "Delimiter", HelpText = "Character(s) used to separate fields", Required = false, Default = ",")]
        public string Delimiter { get; set; }

        [Value(4)]
        [Option('t', longName: "TextEnclosure", HelpText = "for text fields, if supplied, wrap txt values with the supplied character(s)", Required = false, Default = "")]
        public string TextEnclosure { get; set; }

        [Value(5)]
        [Option('q', longName: "QueryFile", HelpText = "path to the file with the query to execute", Required = false, Default ="")]
        public string QueryFile { get; set; }

        [Value(6)]
        [Option('i', longName: "InlineQuery", HelpText = "Text of the query to be executed", Required = false, Default = "")]
        public string InlineQuery { get; set; }

        [Value(7)]
        [Option('x', longName: "CommandTimeout", HelpText = "Command Timeout in seconds", Required = false, Default = 120)]
        public int  CommandTimeout { get; set; }


        [Value(8)]
        [Option('e', longName: "SuppressEmptyFile", HelpText = "Suppress output file if query has no results", Required = false, Default = false)]
        public bool SuppressEmptyFile { get; set; }

        public void Validate()
        {
            if (InlineQuery == "" && QueryFile == "")
            {
                throw new ArgumentException("Either InlineQuery or QueryFile must be provided");
            }
        }
    }
}
