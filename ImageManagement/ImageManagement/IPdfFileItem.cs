using PdfToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement
{
    /// <summary>
    /// PDFファイルの情報インターフェイス
    /// </summary>
    public interface IPdfFileItem: IProcessingModel
    {
        /// <summary>
        /// PDFファイルを読み込み、サムネイルができている状態
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        IPdf PdfItem { get; }

        Image Thumbnail { get; }

        Task<Image> CreateThumbnailAsync(int page = 0);

        Task<Image> CreateThumbnailAsync(Rectangle trimingRectangle, int page = 0);
    }
}
