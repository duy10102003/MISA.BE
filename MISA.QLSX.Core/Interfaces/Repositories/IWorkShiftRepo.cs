using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;

namespace MISA.QLSX.Core.Interfaces.Repositories
{
    public interface IWorkShiftRepo : IBaseRepo<WorkShift>
    {
        /// <summary>
        /// hàm lấy tất cả dữ liệu có phân trang search nhanh lọc 
        /// </summary>
        /// <param name="workShiftFilter"></param>
        /// <returns>danh sách ca làm việc</returns>
        /// created by: DuyLC(29/11/2025)
        Task<PagedResult<WorkShiftListItemDto>> GetPagingAsync(WorkShiftFilterDtoRequest workShiftFilter);

        /// <summary>
        /// lấy ca làm việc theo id
        /// </summary>
        /// <param name="id">id của ca làm việc muốn tìm</param>
        /// <returns>Thông tin của ca làm việc có id trùng khớp</returns>
        /// created by: DuyLC(29/11/2025)
        Task<WorkShiftDetailDto?> GetDetailAsync(Guid id);

        /// <summary>
        /// hàm kiểm tra mã ca làm đã tồn tại hay chưa
        /// </summary>
        /// <param name="workShiftCode">giá trị mã ca làm cần kiểm tra</param>
        /// <param name="excludeId">ID của ca làm việc hiện tại (null khi tạo mới, 
        /// có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>trả vế true/false</returns>
        /// /// created by: DuyLC(29/11/2025)
        Task<bool> CheckCodeExistsAsync(string workShiftCode, Guid? excludeId = null);

        /// <summary>
        /// hàm cập nhật lại trạng thái của ca làm việc (sử dụng, ngưng sử dụng)
        /// </summary>
        /// <param name="ids">danh sách ID mã ca làm việc cần cập nhật trạng thái</param>
        /// <param name="isActive">Trạng thái cần cập nhật: 1: sử dụng 0: ngưng sử dụng</param>
        /// <param name="modifiedBy">ID nhân viên thực hiện cập nhật</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// /// created by: DuyLC(29/11/2025)
        Task<int> UpdateStatusAsync(IEnumerable<Guid> ids, int isActive, Guid modifiedBy);

        /// <summary>
        /// xóa nhiều ca làm việc cùng 1 lúc
        /// </summary>
        /// <param name="ids">danh sách ID mã ca làm việc cần xóa</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// /// created by: DuyLC(29/11/2025)
        Task<int> DeleteManyAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// xóa nhiều ca làm việc cùng 1 lúc
        /// </summary>
        /// <param name="workShiftFilter"></param>
        /// <returns>danh sách ca làm việc</returns>
        /// /// created by: DuyLC(29/11/2025)
        Task<IEnumerable<WorkShiftListItemDto>> GetExportDataAsync(WorkShiftFilterDtoRequest workShiftFilter);
    }
}

