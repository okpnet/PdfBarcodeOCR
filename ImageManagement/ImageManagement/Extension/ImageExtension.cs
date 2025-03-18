using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Extension
{
    public static class ImageExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxSide">一辺の長さ</param>
        /// <returns></returns>
        public static Size ChangeScaleSide(this Image image,int maxSide)
        {
            var resultSize=image.Width > image.Height ?  
                new Size(maxSide, (int)((float)image.Height / (float) maxSide)) : new Size((int)((float)image.Width / (float)maxSide), maxSide);
            return resultSize;
        }
    }
}
