using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;
using SqlToFlatFileLib.Logging;

namespace TestSqlToFlatFile
{
    [TestClass]
    public class TestWriteFileOdbc
    {
        private string _connectionString =
        //@"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\Projectsv13;Database=master;uid=TestOdbc;pwd=TestOdbc;";
        @"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=yes;";
        private static IAppLogger _logger = DefaultLogger.Instance;

        [TestMethod]
        public void OdbcTestWriter()
        {
            var outputFile = "test1Odbc.txt";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = "|"
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            dataWriter.Write();

            var execDir = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void OdbcTestWriterWithDateSuffix_ExplicitDirectory()
        {
            var execDir = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFile = Path.Combine(execDir, "testExplicitWithDate{currentdatetime:format=yyyyMMdd}Odbc.csv");
            var outputFileIntended = "testExplicitWithDate" + DateTime.Now.ToString("yyyyMMdd") + "Odbc.csv";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                //DateSuffixFormat = "yyyyMMdd",
                Delimiter = ","
            };

            var dataWriter = new DataWriter(_logger, writerParams);

            Assert.AreEqual(outputFileIntended,Path.GetFileName(dataWriter.CalculatedOutputFilePath));

            if (File.Exists(dataWriter.CalculatedOutputFilePath))
            {
                File.Delete(dataWriter.CalculatedOutputFilePath);
            }

            dataWriter.Write();

            var outputFileInfo = new FileInfo(dataWriter.CalculatedOutputFilePath);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void OdbcTestWriterWithDateSuffix_ImplicitDirectory()
        {
            var outputFile = "testImplicitWithDateTabs_{currentdatetime:format=yyyyMMdd}Odbc.txt";
            var outputFileIntended = "testImplicitWithDateTabs_" + DateTime.Now.ToString("yyyyMMdd") + "Odbc.txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                //DateSuffixFormat = "yyyyMMdd",
                Delimiter = "\t"
            };



            var dataWriter = new DataWriter(_logger, writerParams);

            Assert.AreEqual(outputFileIntended, Path.GetFileName(dataWriter.CalculatedOutputFilePath));

            if (File.Exists(dataWriter.CalculatedOutputFilePath))
            {
                File.Delete(dataWriter.CalculatedOutputFilePath);
            }

            dataWriter.Write();

            var outputFileInfo = new FileInfo(dataWriter.CalculatedOutputFilePath);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void OdbcTestWriterWithNoDataShouldStillOutputAFile()
        {
            var outputFile = "nofile{currentdatetime:format=yyyyMMdd}Odbc.txt";
            var outputFileIntended = "nofile" + DateTime.Now.ToString("yyyyMMdd") + "Odbc.txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                InlineQuery = "select 1 where 1 = 2",
                OutputFilePath = outputFile,
                Delimiter = "\t"
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            Assert.AreEqual(outputFileIntended, Path.GetFileName(dataWriter.CalculatedOutputFilePath));

            if (File.Exists(dataWriter.CalculatedOutputFilePath))
            {
                File.Delete(dataWriter.CalculatedOutputFilePath);
            }

            dataWriter.Write();

            var outputFileInfo = new FileInfo(dataWriter.CalculatedOutputFilePath);

            Assert.IsTrue(outputFileInfo.Exists);
        }

        [TestMethod]
        public void OdbcTestCommonSqlDataTypesCanBeWritten()
        {
            var outputFile = "testDataTypes_TextEnclosureOdbc.csv";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

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

            var dataWriter = new DataWriter(_logger, writerParams);
            dataWriter.Write();

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);

        }
    }
}
