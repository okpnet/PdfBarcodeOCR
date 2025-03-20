using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public class BarcodeResult : IBarcodeResult
    {
        public Rectangle Rect { get; }

        public string Value { get; }


        public BarcodeResult(Rectangle rect, string value)
        {
            Rect = rect;
            Value = value;
        }

        public BarcodeResult(ZXing.Result readResut)
        {
            Value = readResut.Text;
            Rect = new Rectangle(
                (int)readResut.ResultPoints.Min(t => t.X),
                (int)readResut.ResultPoints.Min(t => t.Y),
                (int)(readResut.ResultPoints.Max(t => t.X) - readResut.ResultPoints.Min(t => t.X)),
                (int)(readResut.ResultPoints.Max(t => t.Y) - readResut.ResultPoints.Min(t => t.Y)));
        }
    }
}
