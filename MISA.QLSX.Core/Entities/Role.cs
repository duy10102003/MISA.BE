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
    [MISATableName("role")]
    public class Role
    {
        [Column("role_id")]
        [Key]
        public Guid RoleId { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }

        [Column("description")]
        public string? RoleDescription { get; set; }
        [Column("is_deleted")]
        public int is_deleted { get; set; }
    }
}
