using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverer.ImageProcessing
{
    public interface IImageDecorator
    {
        public string ImageFilePath { get; }
        public Image Thumbnail { get; }

        public Exception? Exceptions { get; }

        void SetTumbnail(Rectangle trimmingRectangle, int ratio);

        Image GetImage();
    }
}
