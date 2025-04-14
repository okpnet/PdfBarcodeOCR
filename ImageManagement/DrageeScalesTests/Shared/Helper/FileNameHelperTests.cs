using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrageeScales.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Helper.Tests
{
    [TestClass()]
    public class FileNameHelperTests
    {
        [TestMethod()]
        public void CreateNumberAppendToNewnameTest()
        {
            string[] array = ["test", "test", "test_2", "test_3", "test_4", "test_5"];
            var name = FileNameHelper.CreateNumberAppendToNewname(array, "test", "(^_^)");

            Assert.Fail();
        }
    }
}