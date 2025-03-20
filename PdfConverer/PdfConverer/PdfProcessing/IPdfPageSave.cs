using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverer.PdfProcessing
{
    public interface IPdfPageSave : IPdfPage
    {
        bool SaveImage(string filepath, ImageFormat format);
    }
}
