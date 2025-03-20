using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public interface IBarcodeItemException
    {
        BarcodeReadException? Exception { get; }
    }
}
