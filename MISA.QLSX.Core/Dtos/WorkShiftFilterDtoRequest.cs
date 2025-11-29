namespace MISA.QLSX.Core.Dtos
{
    public class WorkShiftFilterDtoRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Keyword { get; set; }

        public List<ColumnFilterDto>? ColumnFilters { get; set; }

        public List<ColumnSortDto>? ColumnSorts { get; set; }
    }
}

