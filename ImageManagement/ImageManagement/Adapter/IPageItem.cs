using ImageManagement.Base;
using PdfConverer.PdfProcessing;

namespace ImageManagement.Adapter
{
    /// <summary>
    /// PDFファイルの情報インターフェイス
    /// </summary>
    public interface IPageItem : IProcessingModel
    {
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        IPdfPage? PdfPages { get; }
        /// <summary>
        /// ファイル名
        /// 保存するときに使用
        /// </summary>
        string FileNameToSave { get; set; }
    }
}
