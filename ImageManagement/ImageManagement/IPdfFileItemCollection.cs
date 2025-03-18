using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement
{
    /// <summary>
    /// PDFファイルの情報のコレクションインターフェイス
    /// </summary>
    public interface IPdfFileItemCollection: IProcessingModel
    {
        Task AddRangeAsyn(IEnumerable<string> filePaths);

        Task AddItem(string filePath);
    }
}
