﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverer.PdfProcessing
{
    public interface IPdfToImage
    {
        Bitmap FromFile(string filename);
    }
}
