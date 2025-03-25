using ImageManagement.Collection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service
{
    public class PdfImageAdapterService
    {
        readonly ILogger? _logger;

        public IPdfFileItemCollection Collection { get; }

        public PdfImageAdapterService()
        {
            Collection = new PdfFileItemCollection();
        }

        public PdfImageAdapterService(ILogger<PdfImageAdapterService> logger):this()=>_logger = logger;
        /// <summary>
        /// 文字列から
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFilesPahh(params string[] paths)
        {
            if(paths.Length == 0)
            {
                yield break;
            }
            foreach (var path in paths)
            {
                switch (GetUsageType(path))
                {
                    case UsageType.File:
                        yield return path;
                        break;
                    case UsageType.Dirictory:
                        foreach (var file in Directory.EnumerateFiles(path, "*.pdf"))
                        {
                            yield return file;
                        }
                        yield break;
                    default:
                        yield break;
                }
            }
        }

        public UsageType GetUsageType(string value)
        {
            if (value is (null or ""))
            {
                return UsageType.None;
            }
            if (System.IO.Directory.Exists(value))
            {
                return UsageType.Dirictory;
            }
            if (System.IO.File.Exists(value))
            {
                return UsageType.File;
            }
            return UsageType.None;
        }
    }
}
