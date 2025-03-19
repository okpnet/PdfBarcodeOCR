using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Extenstion
{
    public static class StringExtension
    {
        public static bool IsFileExists(this string path)
        {
            try
            {
                return File.Exists(path);
            }catch 
            {
                return false; 
            }
        }
    }
}
