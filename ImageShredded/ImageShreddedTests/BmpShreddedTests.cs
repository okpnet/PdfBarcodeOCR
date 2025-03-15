using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageShredded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

namespace ImageShredded.Tests
{
    [TestClass()]
    public class BmpShreddedTests
    {
        [TestMethod()]
        public void GetHorizotalShreddedTest()
        {
            try
            {
                var imagePath = "F:\\data\\barcodetest.bmp";
                var outDir = "F:\\data\\testout";

                var bitmap = new Bitmap(imagePath);
                var list = BmpShredded.GetHorizotalShredded(bitmap, 5);
                var index = 1;
                foreach (var item in list)
                {
                    item.Save(System.IO.Path.Combine(outDir, $"test-{index}.bmp"));
                    index++;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

    }
}