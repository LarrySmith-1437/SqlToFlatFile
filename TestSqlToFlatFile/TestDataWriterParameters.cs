using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;

namespace TestSqlToFlatFile
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
