using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.NetspakleUpdate.Args
{
    public sealed class UpdateEventArg
    {
        internal UpdateState State { get; }

        public bool IsCheckFinished { get; }

        public bool IsCancel { get; set; }

        public UpdateEventArg(UpdateState state, bool isCheckFinished, bool isCancel)
        {
            State = state;
            IsCheckFinished = isCheckFinished;
            IsCancel = isCancel;
        }

        public static UpdateEventArg StandbyUpdate() => new UpdateEventArg(UpdateState.UpdateStandby, false, false);

        public static UpdateEventArg AvailableUpdate()=>new UpdateEventArg(UpdateState.UpdateAvailable, true, false);

        public static UpdateEventArg NotAvailableUpdate() => new UpdateEventArg(UpdateState.UpdateNotAvailable, true, false);


    }
}
