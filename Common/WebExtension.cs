using Nistec.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web
{
    public static class WebExtension
    {
        public static string GetEffectiveRecords(int result)
        {
            if (result > 1)
                return GetDataResult(DataResult.Commit);
            return GetDataResult((DataResult)result);
        }
        public static string GetDataResult(this DataResult result)
        {
            switch (result)
            {
                case DataResult.Error:
                    return "Error";
                case DataResult.None:
                    return "None commited";
                case DataResult.Commit:
                    return "Success";
                default:
                    return "Unknown result";
            }
        }
    }
}
