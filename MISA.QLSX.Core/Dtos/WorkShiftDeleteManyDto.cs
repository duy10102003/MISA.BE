namespace MISA.QLSX.Core.Dtos
{
    public class WorkShiftDeleteManyDto
    {
        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    }
}

