using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfToImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage.Tests
{
    [TestClass()]
    public class PdfItemPageTests
    {
        [TestMethod()]
        public async void SaveImageAsyncTest()
        {
            var dirDirPath = "C:\\Users\\htakahashi\\Downloads\\";
            var files = System.IO.Directory.GetFiles(dirDirPath, "*.pdf");
            if (files is null || files.Length == 0) Assert.Fail();
            foreach(var file in files)
            {
                var pdf = new PdfItem(file);
                await pdf.InitPageItemAsync();
                pdf.
            }
            Assert.Fail();
        }
    }
}