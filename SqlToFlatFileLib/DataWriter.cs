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
    public class DataWriter
    {
        private IAppLogger _appLogger;

        private DataWriterParameters _writerParams;

        public string CalculatedOutputFilePath
        {
            get
            {
                return DateContentRenderer.Render(_writerParams.OutputFilePath, "currentdatetime", DateTime.Now);
            }
        }

        public DataWriter()
        {
            _appLogger = DefaultLogger.Instance;
        }

        public DataWriter(DataWriterParameters writerParameters )
            : this()
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
                _appLogger.Info("begin SqlToFlatFileLib.DataWriter.Write operation");

                _writerParams.LogParameters(_appLogger);

                using (var conn = GetDbConnectionForDatabaseType())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 120;
                        cmd.CommandText = GetQuery();
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();

                        var filename = CalculatedOutputFilePath;

                        _appLogger.Info($"File to be written to: {filename}");
                        File.Delete(filename);

                        StreamWriter fileWriter = null;
                        
                        int recordCounter = 0;
                        while (reader.Read())
                        {
                            if (recordCounter == 0)
                            {
                                fileWriter = InitializeFile(filename);
                                WriteFileHeader(reader, fileWriter);
                            }

                            recordCounter++;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                WriteValue(fileWriter, ReadDataTypes(reader, i), i > 0);
                            }
                            WriteValue(fileWriter, Environment.NewLine, false);
                        }

                        if (recordCounter == 0)
                        {
                            AbortFileWrite(filename);
                            return;
                        }
                        fileWriter?.Dispose();
                        _appLogger.Info($"{recordCounter} records written.");
                    }
                }
            }
            catch (Exception ex)
            {
                _appLogger.Error("File Writing failed due to exception. ", ex);
                throw;
            }
        }

        private void AbortFileWrite(string filename)
        {
            File.Delete(filename);
            _appLogger.Info("No records were returned, aborting file write.");
        }

        private void WriteFileHeader(IDataReader reader, StreamWriter fileWriter)
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

        public string ReadDataTypes(IDataReader reader, int i)
        {
            string contents = "";
            if (!reader.IsDBNull(i))
            {
                switch (reader.GetDataTypeName(i).ToLower())
                {
                    case "dbtype_dbtimestamp":
                    case "datetime":
                        contents = reader.GetDateTime(i).ToString("G");
                        break;
                    case "dbtype_dbdate":
                    case "date":
                        contents = reader.GetDateTime(i).ToString("d");
                        break;
                    case "dbtype_i4":
                    case "int":
                        contents = reader.GetInt32(i).ToString();
                        break;
                    case "dbtype_numeric":
                    case "dbtype_decimal":
                    case "decimal":
                    case "numeric":
                        contents = reader.GetDecimal(i).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "dbtype_bool":
                    case "bit":
                        contents = reader.GetBoolean(i) ? "True" : "False";
                        break;
                    case "tinyint":
                        contents = reader.GetByte(i).ToString();
                        break;
                    case "bigint":
                    case "dbtype_i8":
                        contents = reader.GetInt64(i).ToString();
                        break;
                    case "dbtype_i2":
                    case "smallint":
                        contents = reader.GetInt16(i).ToString();
                        break;
                    case "dbtype_r8":
                        contents = reader.GetDouble(i).ToString(CultureInfo.InvariantCulture);
                        break;
                    default:
                        contents = $"{_writerParams.TextEnclosure}{reader[i].ToString()}{_writerParams.TextEnclosure}";
                        break;
                }
            }
            return contents;
        }

        private string GetQuery()
        {
            if (!String.IsNullOrEmpty(_writerParams.InlineQuery))
                return _writerParams.InlineQuery;

            return File.ReadAllText(_writerParams.QueryFile);
        }
    }
}