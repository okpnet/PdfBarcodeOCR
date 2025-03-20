using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader
{
    public class BarcodeReadException : Exception
    {
        const string MESSAGE = "";
        public BarcodeReadException(Exception ex):base(MESSAGE,ex)
        {
        }
    }
}
