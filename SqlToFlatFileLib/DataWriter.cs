using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlToFlatFileLib.Logging;

namespace SqlToFlatFileLib
{
    public class DataWriter : IDataWriter
    {
        private IAppLogger _logger;

        private DataWriterParameters _writerParams;

        public string CalculatedOutputFilePath
        {
            get
            {
                return DateContentRenderer.Render(_writerParams.OutputFilePath, "currentdatetime", DateTime.Now);
            }
        }

        public DataWriter(IAppLogger logger)
        {
            _logger = logger;
        }

        public DataWriter(IAppLogger logger, DataWriterParameters writerParameters )
            : this(logger)
        {
            _writerParams = writerParameters;
        }

        public IDbConnection GetDbConnectionForDatabaseType()
        {
            switch (_writerParams.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlConnection(_writerParams.ConnectionString);
                case DatabaseType.Odbc:
                    return new OdbcConnection(_writerParams.ConnectionString);
                case DatabaseType.OleDb:
                    return new OleDbConnection(_writerParams.ConnectionString);
                default:
                    throw new ApplicationException("Incorrect database type");
            }
        }

        public void Write()
        {
            try
            {
                _logger.Info("begin SqlToFlatFileLib.DataWriter.Write operation");

                _writerParams.LogParameters(_logger);

                using (var conn = GetDbConnectionForDatabaseType())
                {
                    conn.Open();
                    _logger.Debug("Connection Opened.");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _writerParams.CommandTimeout;
                        cmd.CommandText = GetQuery();
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();

                        var filename = CalculatedOutputFilePath;

                        _logger.Info($"File to be written to: {filename}");
                        File.Delete(filename);

                        StreamWriter fileWriter = null;

                        fileWriter = InitializeFile(filename);
                        WriteFileHeader(reader, fileWriter);

                        int recordCounter = 0;
                        while (reader.Read())
                        {
                            recordCounter++;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                WriteValue(fileWriter, ReadData(reader, i), i > 0);
                            }
                            WriteValue(fileWriter, Environment.NewLine, false);
                        }
                        fileWriter?.Dispose();

                        if (recordCounter == 0)
                        {
                            _logger.Info("No records were returned.");
                            if (_writerParams.SuppressEmptyFile)
                            {
                                _logger.Info("Suppress Empty File, Removing empty output file.");
                                File.Delete(filename);
                            }

                            return;
                        }
                        _logger.Info($"{recordCounter} records written.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("File Writing failed due to exception. ", ex);
                throw;
            }
        }

        public void WriteFileHeader(IDataReader reader, StreamWriter fileWriter)
        {
            if (_writerParams.WriteColNamesAsHeader)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    WriteValue(fileWriter, reader.GetName(i), i > 0);
                }
                WriteValue(fileWriter, Environment.NewLine, false);
            }
        }

        private StreamWriter InitializeFile( string filename)
        {
            StreamWriter fileWriter = File.CreateText(filename);
            return fileWriter;
        }

        public void WriteValue(StreamWriter fileWriter, string content, bool writeDelimiter)
        {
            {
                if (writeDelimiter && _writerParams.Delimiter.Length > 0)
                    fileWriter.Write(_writerParams.Delimiter);

                fileWriter.Write(content);
            }
        }

        public string ReadData(IDataReader reader, int zeroBasedColumnNumber)
        {
            return DatabaseColumnContentFormatter.ReadColumnData(reader, zeroBasedColumnNumber,
                _writerParams.TextEnclosure);
        }

        /// <summary>
        /// If InlineQuery is set, returns the inline query, else attempts to read query from QueryFile.
        /// </summary>
        /// <returns>query text</returns>
        public string GetQuery()
        {
            if (!String.IsNullOrEmpty(_writerParams.InlineQuery))
                return _writerParams.InlineQuery;

            return File.ReadAllText(_writerParams.QueryFile);
        }
    }
}