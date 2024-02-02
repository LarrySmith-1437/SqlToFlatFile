using System;
using Microsoft.Extensions.Logging;

namespace SqlToFlatFileLib
{
    public class DataWriterParameters
    {
        /// <summary>
        /// The connection string to your data source.
        /// Should be appopriate for the DatabaseType.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// File to read the query from.  Will be ignored in favor of InlineQuery if both are provided.
        /// </summary>
        public string QueryFile { get; set; }
        /// <summary>
        /// Sql that will be run. Takes precedence over QueryFile if both are provided.
        /// </summary>
        public string InlineQuery { get; set; }
        /// <summary>
        /// The name of the output file to write to.
        /// Simple filename or full or relative path accepted.
        /// Will accept a date tag in the filename with c# date formatting string
        /// to help write out files daily, hourly, etc.  The tag is {currentdatetime:format=yyyyMMdd}
        ///   for example:
        /// "output{currentdatetime:format=yyyyMMdd}.csv"
        /// </summary>
        public string OutputFilePath { get; set; }
        /// <summary>
        /// True to write the column names as the first row in the output file.
        /// By default, is False, so the column names are NOT written as the first row in the output file.
        /// </summary>
        public bool WriteColNamesAsHeader { get; set; }
        /// <summary>
        /// The delimiter to use between each field in the output file.
        /// </summary>
        public string Delimiter { get; set; }
        /// <summary>
        /// A string to enclose text fields in the output file. Typically single or double quotes.
        /// </summary>
        public string TextEnclosure { get; set; }    
        /// <summary>
        /// The type of database to connect to.
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
        /// <summary>
        /// When true, suppresses (does not write out) the output file if no rows are returned from the query.
        /// </summary>
        public bool SuppressEmptyFile { get; set; }
        /// <summary>
        /// the command/query timeout in seconds
        /// </summary>
        public int CommandTimeout { get; set; }

        public void LogParameters(ILogger appLogger)
        {
            appLogger.LogDebug($"Query File name= {QueryFile}");
            appLogger.LogDebug($"InlineQuery= {InlineQuery}");
            appLogger.LogDebug($"Output File Path= {OutputFilePath}");
            appLogger.LogDebug($"Delimiter= {Delimiter}");
            appLogger.LogDebug($"DatabaseType= {DatabaseType.ToString()}");
            appLogger.LogDebug($"CommmandTimeout= {CommandTimeout}");
            appLogger.LogDebug($"SuppressEmptyFile= {SuppressEmptyFile}");

            string partialConnectionString = "";
            var sections = ConnectionString.Split(';');
                     
            foreach (var section in sections)
            {
                if (section.ToLower().StartsWith("pwd") || section.ToLower().StartsWith("password"))
                {
                    continue;
                }
                partialConnectionString += section + ";";
            }

            appLogger.LogDebug($"Partial Connection String= {partialConnectionString}");
        }

        public void Validate()
        {
            if (String.IsNullOrEmpty(Delimiter))
            {
                throw new ArgumentException("Delimiter is required.");
            }
            if (String.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentException("ConnectionString is required.");
            }
            if (String.IsNullOrEmpty(QueryFile) &&  String.IsNullOrEmpty(InlineQuery))
            {
                throw new ArgumentException("InlineQuery or QueryFile is required. Both cannot be empty.");
            }

            if (String.IsNullOrEmpty(OutputFilePath))
            {
                throw new ArgumentException("OutputFilePath is required.");
            }
        }
    }
}
