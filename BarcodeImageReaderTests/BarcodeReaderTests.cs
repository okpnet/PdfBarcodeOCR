using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarcodeImageReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader.Tests
{
    [TestClass()]
    public class BarcodeReaderTests
    {
        [TestMethod()]
        public void ReadFromFileTest()
        {
            var path = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\300.png";
            var result=BarcodeReader.ReadFromFile(path);
            Assert.Fail();
        }
    }
}