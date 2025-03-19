using System.Drawing;

namespace ImageManagement.Adapter
{
    public class BarcodeResult
    {
        public string Value { get; }

        public bool IsTrimming { get; }

        public Rectangle Rectangles { get; }

        public BarcodeResult(string value)
        {
            Value = value;
            IsTrimming = false;
        }

        public BarcodeResult(string value, Rectangle rectangles):this(value)
        {
            Rectangles = rectangles;
            IsTrimming = true;
        }
    }
}
