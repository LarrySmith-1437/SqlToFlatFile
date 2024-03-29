﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlToFlatFileLib;

namespace TestSqlToFlatFileCore
{
    [TestClass]
    public class TestWriteFileOdbc
    {
        private string _connectionString =
        //@"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\Projectsv13;Database=master;uid=TestOdbc;pwd=TestOdbc;";
        @"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=yes;";
        private static ILogger _logger;

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            _logger = new Mock<ILogger>().Object;
        }

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

            Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void OdbcTestWriterWithDateSuffix_ExplicitDirectory()
        {
            var execDir = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFile = Path.Combine(execDir, "testExplicitWithDate{currentdatetime:format=yyyyMMdd}Odbc.csv");
            var outputFileIntended = "testExplicitWithDate" + DateTime.Now.ToString("yyyyMMdd") + "Odbc.csv";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
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
        public void OdbcTestWriterWithNoDataShouldSuppressEmptyFileIfConfigured()
        {
            var outputFile = "nofile{currentdatetime:format=yyyyMMdd}Odbc.txt";
            var outputFileIntended = "nofile" + DateTime.Now.ToString("yyyyMMdd") + "Odbc.txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                DatabaseType = DatabaseType.Odbc,
                InlineQuery = "select 1 where 1 = 2",
                OutputFilePath = outputFile,
                Delimiter = "\t",
                SuppressEmptyFile = true
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            Assert.AreEqual(outputFileIntended, Path.GetFileName(dataWriter.CalculatedOutputFilePath));

            if (File.Exists(dataWriter.CalculatedOutputFilePath))
            {
                File.Delete(dataWriter.CalculatedOutputFilePath);
            }

            dataWriter.Write();

            var outputFileInfo = new FileInfo(dataWriter.CalculatedOutputFilePath);

            Assert.IsFalse(outputFileInfo.Exists);
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
