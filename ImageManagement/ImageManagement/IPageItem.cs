using PdfToImage;

namespace ImageManagement
{
    /// <summary>
    /// PDFファイルの情報インターフェイス
    /// </summary>
    public interface IPageItem: IProcessingModel
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
