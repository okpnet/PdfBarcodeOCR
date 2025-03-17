using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageShredded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System.Drawing.Imaging;

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
                var imagePath = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\A00441J (2)_1.Bmp";
                var outDir = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\shredded";

                var bitmap = new Bitmap(imagePath);
                //var strechWidth = (int)(bitmap.Width * 2.5);
                //var height = bitmap.Height;
                //using var image = new Bitmap(strechWidth, height);

                //using var grph = Graphics.FromImage(image);
                //grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //grph.DrawImage(bitmap, new Rectangle(0, 0, strechWidth, height));

                //image.Save(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imagePath)!,"strech.png"),ImageFormat.Png);

                var list = BmpShredded.GetHorizotalShredded(bitmap, 6);
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