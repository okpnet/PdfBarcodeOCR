using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public interface IBarcodeItem
    {
        bool HasResultValue { get; }
        bool HasException { get; }
        bool IsSuccessRead { get; }

        bool TryGetResultValue(out IBarcodeResult resultValue);
        bool TryGetException(out BarcodeReadException exception);
    }
}
