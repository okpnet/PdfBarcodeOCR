using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Dtos
{
    public class FileOpenArg : ImageMangementArgBase
    {
        public string[] FileOrDirPaths { get; }

        public FileOpenArg(string[] fileOrDirPaths)
        {
            FileOrDirPaths = fileOrDirPaths;
        }
        public FileOpenArg(IEnumerable<string> fileOrDirPaths)
        {
            FileOrDirPaths = fileOrDirPaths.ToArray();
        }
    }
}
