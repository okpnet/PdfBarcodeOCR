using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Base
{
    /// <summary>
    /// 忙しいモデル
    /// </summary>
    public interface IProcessingModel
    {
        /// <summary>
        /// 処理中のときTrue
        /// </summary>
        bool IsBusy { get; }
    }
}
