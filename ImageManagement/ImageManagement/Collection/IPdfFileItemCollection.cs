using ImageManagement.Adapter;
using ImageManagement.Base;

namespace ImageManagement.Collection
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
