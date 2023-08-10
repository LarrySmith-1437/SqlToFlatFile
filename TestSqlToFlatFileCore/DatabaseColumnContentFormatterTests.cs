using System.Data;
using System.Data.Odbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;

namespace TestSqlToFlatFileCore
{
    [TestClass()]
    public class DatabaseColumnContentFormatterTests
    {

        private static string _connectionString =
            @"Provider=MSDASQL;Driver={Sql Server Native Client 11.0};Server=(localdb)\Projectsv13;Database=master;Trusted_Connection=yes;";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [TestMethod()]
        public void ReadColumnDataTest()
        {
            var query = @"
select *
from
(
select 
    DateTimeType = CAST('2017-06-01 13:55:05.223' as datetime) 
	,DateType = cast('2017-06-15' as date)
)sub
";

            using (var conn = new OdbcConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    reader.Read();
                    var output = DatabaseColumnContentFormatter.ReadColumnData(reader,0,"'");
                    Assert.AreEqual("2017-06-01 13:55:05.223",output);
                }
            }
        }
    }
}