using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Helper
{
    public static class DireictoryHelper
    {
        /// <summary>
        /// ルートDIRを除く、直下のファイルやサブDIRの削除
        /// </summary>
        /// <param name="rootDirectory"></param>
        public static void DeleteFilesAndDirExceptRootDirectory(string rootDirectory)
        {
            if (!System.IO.Directory.Exists(rootDirectory))
            {
                return;
            }
            DirectoryInfo directory = new DirectoryInfo(rootDirectory);
            DeleteFilesAndDirExceptRootDirectory(directory);
        }
        /// <summary>
        /// ルートDIRを除く、直下のファイルやサブDIRの削除
        /// </summary>
        /// <param name="rootDirectory"></param>
        public static void DeleteFilesAndDirExceptRootDirectory(this DirectoryInfo rootDirectory)
        {
            foreach (FileInfo file in rootDirectory.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in rootDirectory.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
