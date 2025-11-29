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
    [MISATableName("work_shift")]
    public class WorkShift
    {
        [Key]
        [Column("work_shift_id")]
        public Guid WorkShiftId { get; set; }

        [Column("work_shift_code")]
        public string WorkShiftCode { get; set; } = string.Empty;

        [Column("work_shift_name")]
        public string WorkShiftName { get; set; } = string.Empty;

        [Column("begin_shift_time")]
        public TimeSpan BeginShiftTime { get; set; }

        [Column("end_shift_time")]
        public TimeSpan EndShiftTime { get; set; }

        [Column("begin_break_time")]
        public TimeSpan? BeginBreakTime { get; set; }

        [Column("end_break_time")]
        public TimeSpan? EndBreakTime { get; set; }

        [Column("working_time")]
        public decimal WorkingTime { get; set; }

        [Column("break_time")]
        public decimal BreakTime { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_active")]
        public int IsActive { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("modified_by")]
        public Guid? ModifiedBy { get; set; }

        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; }
    }
}
