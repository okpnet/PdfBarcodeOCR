using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public class BarcodeItem: IBarcodeItem
    {
        private readonly IBarcodeResult? _resultValue;
        private readonly BarcodeReadException? _barcodeReadException;

        public bool HasException => _barcodeReadException is not null;
        public bool HasResultValue => _resultValue is not null;
        public bool IsSuccessRead=> _resultValue is not null;

        private BarcodeItem(IBarcodeResult? resultValue, BarcodeReadException? barcodeReadException)
        {
            _resultValue = resultValue;
            _barcodeReadException = barcodeReadException;
        }

        public static BarcodeItem FromResult(IBarcodeResult resultValue) => new(resultValue, null);
        public static BarcodeItem FromException(BarcodeReadException barcodeReadException) => new(null, barcodeReadException);
        public static BarcodeItem FromUnableToRead() => new(null, null);
        public bool TryGetResultValue(out IBarcodeResult barcodeItem)
        {
            barcodeItem = default!;
            if (_resultValue is null)
            {
                return false;
            }
            barcodeItem = _resultValue;
            return true;
        }

        public bool TryGetException(out BarcodeReadException barcodeReadException)
        {
            barcodeReadException = default!;
            if (_barcodeReadException is null)
            {
                return false;
            }
            barcodeReadException = _barcodeReadException;
            return true;
        }
    }

}
