using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage
{
    public interface IPdfPageSave:IPdfPage
    {
        Task SaveImageAsync(string filepath, ImageFormat format);
    }
}
