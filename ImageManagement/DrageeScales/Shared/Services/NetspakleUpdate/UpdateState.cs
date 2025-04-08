using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.NetspakleUpdate
{
    public enum UpdateState
    {
        UpdateStandby,//準備完了
        UpdateAvailable,//アップデートあり
        UpdateNotAvailable,//アップデートなし
    }
}
