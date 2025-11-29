namespace MISA.QLSX.Core.Dtos
{
    public class WorkShiftBulkStatusDto
    {
        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();

        public int IsActive { get; set; }

        public Guid ModifiedBy { get; set; }
    }
}

