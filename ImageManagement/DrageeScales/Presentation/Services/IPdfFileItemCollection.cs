using DrageeScales.Core;
using DrageeScales.Views.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Services
{
    /// <summary>
    /// PDFファイルの情報のコレクションインターフェイス
    /// </summary>
    public interface IPdfFileItemCollection : IProcessingModel
    {
        bool IsAny { get; }
        /// <summary>
        /// PDFのページをコレクション
        /// </summary>
        public IEnumerable<PdfPageAdpter> PdfFileItems { get; }

        bool Remove(PdfPageAdpter pdfPageAdpter);

        Task ForeachWhenall(Func<PdfPageAdpter, Task> funcWhenAll);

        Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null);

        Task AddItemAsync(string filePath, IProgress<(int, int)>? progress = null);
    }
}
