using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Exceptions
{
    /// <summary>
    /// Lỗi 404 not found 
    /// Created by: DuyLC(29/11/2025)
    /// </summary>
    public class NotFoundException : BaseException
    {
        public NotFoundException(string devMsg, string? userMsg = null)
           : base(devMsg, userMsg ?? "Không tìm thấy dữ liệu bạn yêu cầu.") { }
    }
}
