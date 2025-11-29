using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Exceptions
{
    public class BaseException : Exception
    {
        public string? UserMsg { get; }
        public string? MoreInfo { get; }

        public BaseException(string devMsg, string? userMsg = null, string? moreInfo = null)
            : base(devMsg)
        {
            UserMsg = userMsg;
            MoreInfo = moreInfo;
        }
    }
}

