using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeImageReader.Shared
{
    public static class TryGetObjectHelper
    {
        public static bool TryGetValue<T>(this T tryValue,out T value)
        {
            value = tryValue;
            if(value is null)
            {
                return false;
            }
            return true;
        }

        public static bool TryGetValue<T>(this T tryValue,int retry,int wait,out T resultValue,Func<T> tryValueFunction)
        {
            if(0>retry || 0 > wait)
            {
                throw new ArgumentOutOfRangeException("'retry' an 'wait' is greater than 0");
            }
            for (var index = 0; index < retry; index++)
            {
                if(tryValueFunction.Invoke().TryGetValue(out T result))
                {
                    resultValue= result;
                    return true;
                }
                if (wait > 0)
                {
                    Task.Delay(wait).Wait();
                }
            }
            resultValue = default!;
            return false;
        }
    }
}
