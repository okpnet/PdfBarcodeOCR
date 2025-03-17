using Microsoft.VisualStudio.TestTools.UnitTesting;
using BarcodeImageReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BarcodeImageReader.Tests
{
    [TestClass()]
    public class BarcodeReaderTests
    {
        [TestMethod()]
        public void ReadFromFileTest()
        {
            var path = "C:\\Users\\htakahashi\\Desktop\\新しいフォルダー\\shredded";
            var files=System.IO.Directory.GetFiles(path);
            if(files is null || files.Count() == 0)
            {
                Assert.Fail();
            }
            var results=new List<string>();
            foreach (var file in files)
            {
                System.Diagnostics.Debug.WriteLine(file);
                using var bitmap=new Bitmap(file);
                var strechWidth = (int)(bitmap.Width * 2.5);
                var height = bitmap.Height;
                using var image = new Bitmap(strechWidth,height);

                using var grph= Graphics.FromImage(image);
                grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                grph.DrawImage(bitmap, new Rectangle(0, 0, strechWidth,height));

                var result = BarcodeReader.ReadFromImage(image);
                if(result is null)
                {
                    continue;
                }
                results.Add(result);
            }
            var a=results.GroupBy((key) => key).Select(t => new { key=t.Key, count=t.Count() }).OrderByDescending(t=>t.count).FirstOrDefault();
            if(results.Count == 0)
            {
                Assert.Fail();
            }
        }
    }
}