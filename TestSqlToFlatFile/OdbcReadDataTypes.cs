using System;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;
using SqlToFlatFileLib.Logging;

namespace TestSqlToFlatFile
{
    [TestClass]
    public class OdbcReadDataTypes
    {
        private static string _connectionString =
        @"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=yes;";

        private static IDataReader _reader;
        private static DataWriter _dataWriter;
        private static IDbConnection _conn;
        private static IAppLogger _logger = DefaultLogger.Instance;


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var outputFile = "testOdbcReadDataTypes.txt";
            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = ",",
                WriteColNamesAsHeader = true,
                TextEnclosure = "'"
            };

            _dataWriter = new DataWriter(_logger, writerParams);

            _conn = _dataWriter.GetDbConnectionForDatabaseType();
            _conn.Open();
            var cmd = _conn.CreateCommand();
            cmd.CommandTimeout = 120;
            cmd.CommandText = _dataWriter.GetQuery();
            cmd.CommandType = CommandType.Text;

            _reader = cmd.ExecuteReader();
            _reader.Read();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try
            {
                _reader?.Close();
                _conn?.Close();
            }
            catch
            {
            }
        }
        [TestMethod]
        public void OdbcDatatype_Binary()
        {
            var value = _dataWriter.ReadData(_reader, _reader.GetOrdinal("BinaryData"));
            Assert.AreEqual("0xD192EA6700", value);
        }
        [TestMethod]
        public void OledbDatatype_VarBinary()
        {
            var value = _dataWriter.ReadData(_reader, _reader.GetOrdinal("VarBinaryData"));
            Assert.AreEqual("0xD192EA67", value);
        }

    }

}
