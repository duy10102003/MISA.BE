using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Dtos
{
    /// <summary>
    /// DTO cho filter theo cột
    /// </summary>
    public class ColumnFilterDto
    {
        /// <summary>
        /// Tên cột cần filter 
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Toán tử filter
        /// Text: contains, not_contains, equals, not_equals, empty, not_empty, starts_with, ends_with
        /// Time/Number: greater_than, less_than, equals, not_equals, greater_or_equal, less_or_equal
        /// </summary>
        public string Operator { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị để filter (có thể null cho empty/not_empty)
        /// </summary>
        public string? Value { get; set; }
    }
}
