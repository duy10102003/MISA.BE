namespace MISA.QLSX.Core.Dtos
{
    public class WorkShiftDetailDto
    {
        public Guid WorkShiftId { get; set; }

        public string WorkShiftCode { get; set; } = string.Empty;

        public string WorkShiftName { get; set; } = string.Empty;

        public TimeSpan BeginShiftTime { get; set; }

        public TimeSpan EndShiftTime { get; set; }

        public TimeSpan? BeginBreakTime { get; set; }

        public TimeSpan? EndBreakTime { get; set; }

        public decimal WorkingTime { get; set; }

        public decimal BreakTime { get; set; }

        public int IsActive { get; set; }

        public string? Description { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}

