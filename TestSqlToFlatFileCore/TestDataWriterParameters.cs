using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;

namespace TestSqlToFlatFileCore
{
    [TestClass]
    public class TestDataWriterParameters
    {
        [TestMethod]
        [ExpectedException (typeof(ArgumentException))]
        public void InsufficientParametersShouldFailValidation()
        {
            var parameters = new DataWriterParameters();
            parameters.Validate();
        }
    }
}
