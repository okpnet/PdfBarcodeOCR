using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement
{
    public class PdfFileItemCollection:ObservableCollection<IPdfFileItem>, IParentPdfFileItem, IPdfFileItemCollection
    {
        /// <summary>
        /// サムネイルの一辺の最大長さ
        /// </summary>
        public int ThumbnailSide { get; set; }

        public IEnumerable<IPdfFileItem> PdfFileItems => this;

        public async Task AddItem(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return ;
            var addItem = new PdfFileIem(this, filePath);
            Add(addItem);
            await addItem.InitAsync();
        }

        public async Task AddRangeAsyn(IEnumerable<string> filePaths)
        {
            var tasks=new List<Task>();
            foreach (var filePath in filePaths)
            {
                tasks.Add(AddItem(filePath));
            }
            await Task.WhenAll(tasks);
        }
    }
}
