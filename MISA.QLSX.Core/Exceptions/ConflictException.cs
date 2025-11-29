using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Exceptions
{
    /// <summary>
    /// 409 conflict dữ liệu
    /// Created: DuyLC(29/11/2025)
    /// </summary>
    public class ConflictException : BaseException
    {
        public ConflictException(string devMsg, string? userMsg = null)
            : base(devMsg, userMsg ?? "Dữ liệu đã tồn tại trong hệ thống.") { }
    }
}
