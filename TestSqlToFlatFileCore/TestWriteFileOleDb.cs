using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlToFlatFileLib;

namespace TestSqlToFlatFileCore
{
    [TestClass]
    public class TestWriteFileOleDb
    {
        private string _connectionString =
            @"Provider=SQLNCLI11;Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=yes;";
        private static ILogger _logger;

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            _logger = new Mock<ILogger>().Object;
        }

        [TestMethod]
        public void OleDbTestWriter()
        {
            var outputFile = "test1OleDb.txt";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.OleDb,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = "|"
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            dataWriter.Write();

            Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void OleDbTestWriterWithDateSuffix_ExplicitDirectory()
        {
            var execDir = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFile = Path.Combine(execDir, "testExplicitWithDate{currentdatetime:format=yyyyMMdd}OleDb.csv");
            var outputFileIntended = "testExplicitWithDate" + DateTime.Now.ToString("yyyyMMdd") + "OleDb.csv";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.OleDb,
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
        public void OleDbTestWriterWithDateSuffix_ImplicitDirectory()
        {
            var outputFile = "testImplicitWithDateTabs_{currentdatetime:format=yyyyMMdd}OleDb.txt";
            var outputFileIntended = "testImplicitWithDateTabs_" + DateTime.Now.ToString("yyyyMMdd") + "OleDb.txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.OleDb,
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
        public void OleDbTestWriterWithNoDataShouldStillOutputAFile()
        {
            var outputFile = "nofile{currentdatetime:format=yyyyMMdd}OleDb.txt";
            var outputFileIntended = "nofile" + DateTime.Now.ToString("yyyyMMdd") + "OleDb.txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.OleDb,
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
        public void OleDbTestCommonSqlDataTypesCanBeWritten()
        {
            var outputFile = "testDataTypes_TextEnclosureOleDb.csv";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.OleDb,
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
