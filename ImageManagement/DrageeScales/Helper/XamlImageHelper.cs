using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DrageeScales.Helper
{
    public static class XamlImageHelper
    {
        public static BitmapImage ConvertImage(this Image image, ImageFormat imageFormat) 
        {
            using var stream = new InMemoryRandomAccessStream();
            image.Save(stream.AsStream(), imageFormat);
            stream.Seek(0);
            var result = new BitmapImage();
            result.SetSource(stream);
            return result;
        }
    }
}
