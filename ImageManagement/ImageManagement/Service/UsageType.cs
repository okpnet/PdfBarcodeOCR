using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service
{
    public enum UsageType
    {
        [DebuggerDisplay("Not applicable")]
        None,
        [DebuggerDisplay("The item is File")]
        File,
        [DebuggerDisplay("The item is Dirictory or Folder")]
        Dirictory,
    }
}
