using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage.Tests
{
    [TestClass()]
    public class PdfItemTests
    {
        [TestMethod()]
        public async Task PdfItemTest()
        {
            var dirDirPath = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\";
            var files = System.IO.Directory.GetFiles(dirDirPath, "*.pdf");
            if (files is null || files.Length == 0) Assert.Fail();
            foreach (var file in files)
            {
                var pdf = new PdfItem(file);
                await pdf.InitPageItemAsync();

                foreach (var pdfPage in pdf.Pages.OfType<IPdfPageSave>())
                {
                    await pdfPage.SaveImageAsync(System.IO.Path.Combine(dirDirPath, $"{System.IO.Path.GetFileNameWithoutExtension(pdf.FilePath)}_{pdfPage.PageNumber+1}.{ImageFormat.Bmp.ToString()}"), ImageFormat.Bmp);
                }
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void InitPageItemAsyncTest()
        {
            //トリミング

            Assert.Fail();
        }

        [TestMethod()]
        public void FromFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }
}