using ImageManagement.Base;
using PdfToImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Collection
{
    /// <summary>
    /// PDFファイルの情報のコレクションインターフェイス
    /// </summary>
    public interface IPdfFileItemCollection : IProcessingModel
    {
        Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null);

        Task AddItem(string filePath, IProgress<(int, int)>? progress = null);
    }
}
