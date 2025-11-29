using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Interfaces.Repositories
{
    /// <summary>
    /// Base repo interface cho CRUD cơ bản
    /// </summary>
    /// Created By: DuyLC (29/11/2025)
    /// <typeparam name="T">Thực thể cần truyền vào VD: workshift, role, employee</typeparam>
    public interface  IBaseRepo<T> where T : class
    {
        /// <summary>
        /// Hàm lấy danh sách tất cả dữ liệu
        /// </summary>
        /// <returns>trả về danh sách tất cả dữ liệu</returns>
        /// Created By: DuyLC (29/11/2025)
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Hàm lấy thông tin chi tiết theo id
        /// </summary>
        /// <param name="id"> id của đối tượng mà mình muốn tìm </param>
        /// <returns>đối tượng có id phù hợp</returns>
        /// Created By: DuyLC (29/11/2025)
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Hàm thêm mới bản ghi trong database
        /// </summary>
        /// <param name="entity">thuộc tính của thực thể mình muốn thêm</param>
        /// <returns>trả về số dòng bị ảnh hưởng </returns>
        /// Created By: DuyLC (29/11/2025)
        Task<Guid> InsertAsync(T entity);

        /// <summary>
        /// Hàm cập nhật 1 bản ghi theo id
        /// </summary>
        /// <param name="id"> id của bản ghi mình muốn cập nhật</param>
        /// <param name="entity">thuộc tính của thực thể</param>
        /// <returns>số dòng bị ảnh hưuongr</returns>
        /// Created By: DuyLC (29/11/2025)
        Task<int> UpdateAsync(Guid id, T entity);

        /// <summary>
        /// xóa mềm 1 bản ghi trong database
        /// </summary>
        /// <param name="id">id mà mình muốn xóa </param>
        /// <returns>số dòng bị ảnh hưởng </returns>
        /// Created By: DuyLC (29/11/2025)
        Task<int> DeleteAsync(Guid id);

    }
}
