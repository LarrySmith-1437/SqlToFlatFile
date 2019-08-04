using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;
using SqlToFlatFileLib.Logging;

namespace TestSqlToFlatFile
{
    [TestClass]
    public class TestWriteFileSqlServer
    {
        private string _connectionString =
            @"Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=True;";
        private static IAppLogger _logger = DefaultLogger.Instance;

        [TestMethod]
        public void TestWriter()
        {
            var outputFile = "test1.txt";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = "|",
                DatabaseType = DatabaseType.SqlServer
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            dataWriter.Write();

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);
        }

        [TestMethod]
        public void TestWriterWithDateSuffix_ExplicitDirectory()
        {
            var execDir = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var outputFile = Path.Combine(execDir, "testExplicitWithDate{currentdatetime:format=yyyyMMdd}.csv");
            var outputFileIntended = "testExplicitWithDate" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                //DateSuffixFormat = "yyyyMMdd",
                Delimiter = ",",
                DatabaseType = DatabaseType.SqlServer

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
        public void TestWriterWithDateSuffix_ImplicitDirectory()
        {
            var outputFile = "testImplicitWithDateTabs_{currentdatetime:format=yyyyMMdd}.txt";
            var outputFileIntended = "testImplicitWithDateTabs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = "\t",
                DatabaseType = DatabaseType.SqlServer
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
        public void TestWriterWithNoDataShouldStillOutputAFile()
        {
            var outputFile = "nofile{currentdatetime:format=yyyyMMdd}.txt";
            var outputFileIntended = "nofile" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                InlineQuery = "select col1 = 1 where 1 = 2",
                OutputFilePath = outputFile,
                WriteColNamesAsHeader = true,
                Delimiter = "\t",
                DatabaseType = DatabaseType.SqlServer
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
            Assert.AreEqual("col1\r\n", File.ReadAllText(outputFileInfo.FullName));
        }

        [TestMethod]
        public void TestCommonSqlDataTypesCanBeWritten()
        {
            var outputFile = "testDataTypes_TextEnclosure.csv";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                QueryFile = "ReturnAllCommonDataTypesQuery.sql",
                OutputFilePath = outputFile,
                Delimiter = ",",
                WriteColNamesAsHeader = true,
                TextEnclosure = "'",
                DatabaseType = DatabaseType.SqlServer
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            dataWriter.Write();

            var outputFileInfo = new FileInfo(outputFile);

            Assert.IsTrue(outputFileInfo.Exists);
            Assert.IsTrue(outputFileInfo.Length > 10);

        }

        [TestMethod]
        public void TestQueryThatGeneratesError()
        {
            var outputFile = "testErrorQuery.csv";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var writerParams = new DataWriterParameters
            {
                ConnectionString = _connectionString,
                InlineQuery = "Select firstcol = 1/0",
                OutputFilePath = outputFile,
                Delimiter = ",",
                WriteColNamesAsHeader = true,
                TextEnclosure = "'",
                DatabaseType = DatabaseType.SqlServer
            };

            var dataWriter = new DataWriter(_logger, writerParams);
            Assert.ThrowsException<SqlException>(delegate
            {
                dataWriter.Write();
            });
            //var outputFileInfo = new FileInfo(outputFile);

            //Assert.IsTrue(outputFileInfo.Exists);
            //Assert.IsTrue(outputFileInfo.Length > 5);

        }

    }
}
