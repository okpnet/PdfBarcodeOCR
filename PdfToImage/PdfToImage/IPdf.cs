﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage
{
    public interface IPdf
    {
        DpiType Dpi { get; set; }

        IEnumerable<IPdfPage> Pages { get; }

        IPdfPage? this[int pageIndex] { get; }
    }
}
