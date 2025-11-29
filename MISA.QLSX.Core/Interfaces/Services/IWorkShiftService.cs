using MISA.QLSX.Core.Dtos;

namespace MISA.QLSX.Core.Interfaces.Services
{
    public interface IWorkShiftService
    {
        Task<PagedResult<WorkShiftListItemDto>> GetPagingAsync(WorkShiftFilterDtoRequest request);

        Task<WorkShiftDetailDto> GetByIdAsync(Guid id);

        Task<Guid> CreateAsync(WorkShiftCreateDto request);

        Task<int> UpdateAsync(Guid id, WorkShiftUpdateDto request);

        Task<int> DeleteAsync(Guid id);

        Task<int> DeleteManyAsync(IEnumerable<Guid> ids);

        Task<int> UpdateStatusAsync(WorkShiftBulkStatusDto request);

        Task<IEnumerable<WorkShiftListItemDto>> GetExportDataAsync(WorkShiftFilterDtoRequest request);

        Task<bool> CheckCodeExistsAsync(string code, Guid? excludeId);
    }
}

