using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplexcel;
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
        
        public void Write()
        {
            try
            {
                _appLogger.Info("begin SqlToFlatFileLib.DataWriter.Write operation");

                _writerParams.LogParameters(_appLogger);

                using (var conn = new SqlConnection(_writerParams.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 120;
                        cmd.CommandText = GetQuery();
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            _appLogger.Info("No records were returned, aborting file write.");
                            return;
                        }

                        var filename = CalculatedOutputFilePath;

                        _appLogger.Info($"File to be written to: {filename}");
                        File.Delete(filename);

                        using (var fileWriter = File.CreateText(filename))
                        {
                            if (_writerParams.WriteColNamesAsHeader)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    WriteValue(fileWriter, reader.GetName(i), i > 0);
                                }
                                WriteValue(fileWriter, Environment.NewLine, false);
                            }

                            int recordCounter = 0;
                            while (reader.Read())
                            {
                                recordCounter++;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    WriteValue(fileWriter, ReadDataTypes(reader, i), i > 0);
                                }
                                WriteValue(fileWriter, Environment.NewLine, false);
                            }
                            _appLogger.Info($"{recordCounter} records written.");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _appLogger.Error("File Writing failed due to exception. ", ex);
                throw;
            }
        }

        public void WriteValue(StreamWriter fileWriter, string content, bool writeDelimiter)
        {
            {
                if (writeDelimiter && _writerParams.Delimiter.Length > 0)
                    fileWriter.Write(_writerParams.Delimiter);

                fileWriter.Write(content);
            }
        }

        public string ReadDataTypes(SqlDataReader reader, int i)
        {
            string contents = "";
            if (!reader.IsDBNull(i))
            {
                switch (reader.GetDataTypeName(i).ToLower())
                {
                    case "datetime":
                        contents = reader.GetDateTime(i).ToString("G");
                        break;
                    case "date":
                        contents = reader.GetDateTime(i).ToString("d");
                        break;
                    case "int":
                        contents = reader.GetInt32(i).ToString();
                        break;
                    case "decimal":
                        contents = reader.GetDecimal(i).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "bit":
                        contents = reader.GetBoolean(i) ? "True" : "False";
                        break;
                    case "tinyint":
                        contents = reader.GetByte(i).ToString();
                        break;
                    case "bigint":
                        contents = reader.GetInt64(i).ToString();
                        break;
                    case "smallint":
                        contents = reader.GetInt16(i).ToString();
                        break;
                    default:
                        contents = $"{_writerParams.TextEnclosure}{reader.GetString(i)}{_writerParams.TextEnclosure}";
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