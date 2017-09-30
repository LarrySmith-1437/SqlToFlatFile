using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToFlatFileLib;

namespace TestSqlToXlsx
{
    [TestClass]
    public class TestDateContentRenderer
    {
        [TestMethod]
        public void TestRenderDate()
        {
            var testInput = "This is a formatted date: {currentdate:format=yyMMdd}";
            var output = DateContentRenderer.Render(testInput, "currentdate", new DateTime(2017,07,10,13,05,09));
            Assert.AreEqual("This is a formatted date: 170710", output);
        }

        [TestMethod]
        public void TestRenderDateWithDefaultFormat()
        {
            var testInput = "This is a formatted date: {currentdate}";
            var output = DateContentRenderer.Render(testInput, "currentdate", new DateTime(2017, 07, 10, 13, 05, 09));
            Assert.AreEqual("This is a formatted date: 20170710", output);
        }

        [TestMethod]
        public void TestRenderDateWithTime()
        {
            var testInput = "This is a formatted date: {currentdate:format=MM/dd/yyyy HH:mm:ss}";
            var output = DateContentRenderer.Render(testInput, "currentdate", new DateTime(2017, 07, 10, 13, 05, 09));
            Assert.AreEqual("This is a formatted date: 07/10/2017 13:05:09", output);
        }
    }
}
