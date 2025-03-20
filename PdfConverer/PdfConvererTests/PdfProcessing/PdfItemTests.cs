using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfConverer.PdfProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverer.PdfProcessing.Tests
{
    [TestClass()]
    public class PdfItemTests
    {
        [TestMethod()]
        public void PdfItemTest()
        {
            var dirDirPath = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\";
            var files = System.IO.Directory.GetFiles(dirDirPath, "*.pdf");
            if (files is null || files.Length == 0) Assert.Fail();

            foreach (var file in files)
            {
                var pdf = new PdfItem(file);
                var index = 0;
                foreach (var image in pdf.AllPageImages().OfType<IPdfPageSave>())
                {
                    index += 1;
                    image.SaveImage(System.IO.Path.Combine(dirDirPath, $"{System.IO.Path.GetFileNameWithoutExtension(pdf.FilePath)}_{index}.{ImageFormat.Bmp.ToString()}"), ImageFormat.Bmp);
                }
            }
        }
    }
}