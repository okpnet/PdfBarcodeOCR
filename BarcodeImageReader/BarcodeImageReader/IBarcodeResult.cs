using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public interface IBarcodeResult
    {
        Rectangle Rect { get; }

        string Value { get; }
    }
}
