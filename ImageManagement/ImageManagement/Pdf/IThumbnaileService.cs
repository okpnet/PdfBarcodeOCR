using ImageManagement.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Pdf
{
    public interface IThumbnaileService : IImageFacade
    {
        Image Create();
    }
}
