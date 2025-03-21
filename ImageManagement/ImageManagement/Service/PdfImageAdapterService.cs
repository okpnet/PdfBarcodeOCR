using ImageManagement.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service
{
    public class PdfImageAdapterService
    {
        public IPdfFileItemCollection Collection { get; }

        public PdfImageAdapterService()
        {
            Collection = new PdfFileItemCollection();
        }
    }
}
