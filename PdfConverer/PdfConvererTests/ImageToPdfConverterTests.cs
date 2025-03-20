using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfConverer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverer.Tests
{
    [TestClass()]
    public class ImageToPdfConverterTests
    {
        [TestMethod()]
        public void SaveImageToPdfTest()
        {
            var imagePath = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\A76832I_1.Bmp";
            var pdfPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imagePath)!, "test.pdf");
            using var image=new Bitmap(imagePath);
            image.SaveImageToPdf(pdfPath,100.0f);
            //Assert.Fail();
        }
    }
}