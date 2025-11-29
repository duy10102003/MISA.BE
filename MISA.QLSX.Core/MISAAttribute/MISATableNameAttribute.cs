using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.MISAAttribute
{
    /// <summary>
    /// Setup mapping tên bảng trong database
    /// Created by: TuanDQ (15/11/2025)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MISATableNameAttribute : Attribute
    {
        public string TableName { get; set; }
        public MISATableNameAttribute(string tableName)
        {
               TableName = tableName;  
        }
    }
}
