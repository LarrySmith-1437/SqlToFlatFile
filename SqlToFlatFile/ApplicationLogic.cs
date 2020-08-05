using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SqlToFlatFileLib;

namespace SqlToFlatFile
{
    public class ApplicationLogic
    {
        private readonly ILogger _logger;
        public ApplicationLogic(ILogger<ApplicationLogic> logger)
        {
            _logger = logger;

        }

        public void Run(Options parsedOptions)
        {
            try
            {

                parsedOptions.Validate();

                LogCommandLineArgs(parsedOptions);
                var writerParams = new DataWriterParameters
                {
                    ConnectionString = parsedOptions.ConnectionString,
                    Delimiter = parsedOptions.Delimiter,
                    OutputFilePath = parsedOptions.OutputFilePath,
                    InlineQuery = parsedOptions.InlineQuery,
                    QueryFile = parsedOptions.QueryFile,
                    TextEnclosure = parsedOptions.TextEnclosure,
                    WriteColNamesAsHeader = parsedOptions.Header,
                    CommandTimeout = parsedOptions.CommandTimeout,
                    SuppressEmptyFile = parsedOptions.SuppressEmptyFile
                };

                var dataWriter = new DataWriter(_logger, writerParams);

                dataWriter.Write();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred:");
                Environment.Exit(-1);
            }

        }
        private void LogCommandLineArgs(Options parsedOptions)
        {

            _logger.LogInformation("parsed command line arguments");
            var connbld = new SqlConnectionStringBuilder(parsedOptions.ConnectionString);
            var partialConn = $"DataSource={connbld.DataSource};InitialCatalog={connbld.InitialCatalog}";
            _logger.LogInformation($"   ConnectionString     = {partialConn}");
            _logger.LogInformation($"   OutputFilePath       = {parsedOptions.OutputFilePath}");
            _logger.LogInformation($"   QueryFile            = {parsedOptions.QueryFile}");
            _logger.LogInformation($"   Header               = {parsedOptions.Header }");
            _logger.LogInformation($"   Delimiter            = {parsedOptions.Delimiter }");
            _logger.LogInformation($"   Text Enclosure       = {parsedOptions.TextEnclosure}");
        }

    }
}
