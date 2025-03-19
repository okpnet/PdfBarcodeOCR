using ImageManagement.Base;
using PdfToImage;

namespace ImageManagement.Adapter
{
    /// <summary>
    /// PDFファイルの情報インターフェイス
    /// </summary>
    public interface IPageItem : IProcessingModel
    {
        /// <summary>
        /// PDFファイルを読み込み、サムネイルができている状態
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        IPdfPage? PdfPages { get; }
    }
}
