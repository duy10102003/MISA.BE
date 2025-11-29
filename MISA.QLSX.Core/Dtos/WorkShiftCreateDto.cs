namespace MISA.QLSX.Core.Dtos
{
    public class WorkShiftCreateDto
    {
        public string WorkShiftCode { get; set; } = string.Empty;

        public string WorkShiftName { get; set; } = string.Empty;

        public TimeSpan BeginShiftTime { get; set; }

        public TimeSpan EndShiftTime { get; set; }

        public TimeSpan? BeginBreakTime { get; set; }

        public TimeSpan? EndBreakTime { get; set; }

        public string? Description { get; set; }

        public Guid CreatedBy { get; set; }
    }
}

