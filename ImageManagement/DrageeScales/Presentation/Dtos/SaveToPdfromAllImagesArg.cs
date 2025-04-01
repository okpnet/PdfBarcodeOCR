using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Dtos
{
    public class SaveToPdfromAllImagesArg : ImageMangementArgBase
    {
        public string OutDir { get; }

        public SaveToPdfromAllImagesArg(string outDir)
        {
            OutDir = outDir;
        }
    }
}
