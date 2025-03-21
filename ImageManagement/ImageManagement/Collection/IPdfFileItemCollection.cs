using ImageManagement.Base;

namespace ImageManagement.Collection
{
    /// <summary>
    /// PDFファイルの情報のコレクションインターフェイス
    /// </summary>
    public interface IPdfFileItemCollection : IProcessingModel
    {
        Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null);

        Task AddItemAsync(string filePath, IProgress<(int, int)>? progress = null);
    }
}
