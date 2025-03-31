using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service.Argments
{
    public class FileOpenArg: ImageMangementArgBase
    {
        public string[] FileOrDirPaths { get; }

        public FileOpenArg(string[] fileOrDirPaths)
        {
            FileOrDirPaths = fileOrDirPaths;
        }
    }
}
