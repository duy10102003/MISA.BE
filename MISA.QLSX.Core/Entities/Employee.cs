using MISA.QLSX.Core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Entities
{
    [MISATableName("employee")]
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("employee_full_name")]
        public string EmployeeFullname { get; set; }

        [Column("role_id")]
        public Guid RoleId { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; }
    }
}
