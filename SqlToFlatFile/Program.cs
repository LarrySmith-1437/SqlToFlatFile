using System;
using System.Data.SqlClient;
using CommandLine;
using SqlToFlatFileLib.Logging;
using SqlToFlatFileLib;

namespace SqlToFlatFile
{
    class Program
    {
        /* sample command line
        -w "ISO with Reseller Info" -c "Server=intreport1;Database=aprivapos;Trusted_Connection=True;" -q "..\..\..\TestSqlToFlatFile\IsoWithResellerQuery.sql" -o "test1.xlsx"
        */
        static void Main(string[] args)
        {
            IAppLogger logger = DefaultLogger.Instance;

            logger.Info("");
            logger.Info("------ Begining Execution ---------");

            try
            {
                Options parsedOptions = null;
                var optionResult = Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options => parsedOptions = options);                

                if (null == parsedOptions)
                {
                    logger.Fatal("Application could not parse the command line parameters provided");
                    Environment.Exit(-1);
                }

                parsedOptions.Validate();

                LogCommandLineArgs(logger, parsedOptions);
                var writerParams = new DataWriterParameters
                {
                    ConnectionString = parsedOptions.ConnectionString,
                    Delimiter = parsedOptions.Delimiter,
                    OutputFilePath = parsedOptions.OutputFilePath,
                    InlineQuery = parsedOptions.InlineQuery,
                    QueryFile = parsedOptions.QueryFile,
                    TextEnclosure = parsedOptions.TextEnclosure,
                    WriteColNamesAsHeader = parsedOptions.Header
                };

                var dataWriter = new DataWriter(logger, writerParams);

                dataWriter.Write();
            }
            catch (Exception ex)
            {
                logger.Error("Exception occurred:",ex);
                Environment.Exit(-1);
            }
        }
        private static void LogCommandLineArgs(IAppLogger logger, Options parsedOptions)
        {

            logger.Info("parsed command line arguments");
            var connbld = new SqlConnectionStringBuilder(parsedOptions.ConnectionString);
            var partialConn = $"DataSource={connbld.DataSource};InitialCatalog={connbld.InitialCatalog}";
            logger.Info($"   ConnectionString     = {partialConn}");
            logger.Info($"   OutputFilePath       = {parsedOptions.OutputFilePath}");
            logger.Info($"   QueryFile            = {parsedOptions.QueryFile}");
            //logger.Info($"   DateTimeSuffixFormat = {GetDateSuffixFormat(parsedOptions) }");
            logger.Info($"   Header               = {parsedOptions.Header }");
            logger.Info($"   Delimiter            = {parsedOptions.Delimiter }");
            logger.Info($"   Text Enclosure       = {parsedOptions.TextEnclosure}");
        }

        //private static string GetDateSuffixFormat(Options options)
        //{
        //    return String.IsNullOrWhiteSpace(options.DateSuffixFormat) ? "" : options.DateSuffixFormat;
        //}
    }
}
