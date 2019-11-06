using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SqlToFlatFileLib
{
    public class DataWriterParameters
    {
        public string ConnectionString { get; set; }
        public string QueryFile { get; set; }
        public string InlineQuery { get; set; }
        public string OutputFilePath { get; set; }
        public bool WriteColNamesAsHeader { get; set; }
        public string Delimiter { get; set; }
        public string TextEnclosure { get; set; }    
        public DatabaseType DatabaseType { get; set; }

        public bool SuppressEmptyFile { get; set; }

        public int CommandTimeout { get; set; }

        public void LogParameters(Logging.IAppLogger appLogger)
        {
            appLogger.Debug($"Query File name= {QueryFile}");
            appLogger.Debug($"InlineQuery= {InlineQuery}");
            appLogger.Debug($"Output File Path= {OutputFilePath}");
            appLogger.Debug($"Delimiter= {Delimiter}");
            appLogger.Debug($"DatabaseType= {DatabaseType.ToString()}");
            appLogger.Debug($"CommmandTimeout= {CommandTimeout}");
            appLogger.Debug($"SuppressEmptyFile= {SuppressEmptyFile}");

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

            appLogger.Debug($"Partial Connection String= {partialConnectionString}");
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
