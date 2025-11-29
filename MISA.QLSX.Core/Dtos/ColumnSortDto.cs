using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Dtos
{
    /// <summary>
    /// DTO cho sort theo cột
    /// </summary>
    public class ColumnSortDto
    {
        /// <summary>
        /// Tên cột cần sort 
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;


        /// <summary>
        /// Giá trị để sort (asc, desc)
        /// </summary>
        public string SortDirection { get; set; } = string.Empty;
    }
}
