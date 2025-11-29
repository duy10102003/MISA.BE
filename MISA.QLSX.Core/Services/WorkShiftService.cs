using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Interfaces.Repositories;
using MISA.QLSX.Core.Interfaces.Services;

namespace MISA.QLSX.Core.Services
{
    public class WorkShiftService : IWorkShiftService
    {
        private readonly IWorkShiftRepo _workShiftRepo;
        private readonly IEmployeeRepo _employeeRepo;

        public WorkShiftService(IWorkShiftRepo workShiftRepo, IEmployeeRepo employeeRepo)
        {
            _workShiftRepo = workShiftRepo;
            _employeeRepo = employeeRepo;
        }
        /// <summary>
        /// hàm lấy tất cả dữ liệu có phân trang search nhanh lọc 
        /// </summary>
        /// <param name="workShiftFilter"></param>
        /// <returns>danh sách ca làm việc</returns>
        /// created by: DuyLC(29/11/2025)
        public Task<PagedResult<WorkShiftListItemDto>> GetPagingAsync(WorkShiftFilterDtoRequest workShiftFilter)
        {
            var result = _workShiftRepo.GetPagingAsync(workShiftFilter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách");
            }
            return result;
        }

        /// <summary>
        /// lấy ca làm việc theo id
        /// </summary>
        /// <param name="id">id của ca làm việc muốn tìm</param>
        /// <returns>Thông tin của ca làm việc có id trùng khớp</returns>
        /// <exception cref="NotFoundException">trả về lỗi 404 not found nếu không có data</exception>
        /// created by: DuyLC(29/11/2025)
        public async Task<WorkShiftDetailDto> GetByIdAsync(Guid id)
        {
            // tìm ca làm việc theo id
            var result = await _workShiftRepo.GetDetailAsync(id);
            // trả về 404 nếu k tìm thấy
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy giá trị phù hợp.");
            }

            return result;
        }


        /// <summary>
        /// hàm insert ca làm việc mới 
        /// </summary>
        /// <param name="workShift">đối tượng ca làm việc mới</param>
        /// <returns> trả về id của ca làm việc mới vừa tạo</returns>
        /// created by: DuyLC(29/11/2025)
        public async Task<Guid> CreateAsync(WorkShiftCreateDto workShift)
        {
            //check validate của thông tin được nhập
            ValidateShiftPayload(workShift.WorkShiftCode, workShift.WorkShiftName,
                workShift.BeginShiftTime, workShift.EndShiftTime, workShift.BeginBreakTime, 
                workShift.EndBreakTime);

            // kiểm tra mã ca làm việc có bị trùng hay không
            await EnsureCodeUnique(workShift.WorkShiftCode, null);
            // kiểm tra nhân viên có tồn tại không
            await EnsureEmployeeExists(workShift.CreatedBy);

            //tính toán số giờ của ca làm, số giờ nghỉ
            var (workingTime, breakTime) = CalculateHours(workShift.BeginShiftTime,
                workShift.EndShiftTime, workShift.BeginBreakTime, workShift.EndBreakTime);

            // set giá trị cho đối tượng cần tạo mới
            var workShiftNew = new WorkShift
            {
                WorkShiftId = Guid.NewGuid(),
                WorkShiftCode = workShift.WorkShiftCode.Trim(),
                WorkShiftName = workShift.WorkShiftName.Trim(),
                BeginShiftTime = workShift.BeginShiftTime,
                EndShiftTime = workShift.EndShiftTime,
                BeginBreakTime = workShift.BeginBreakTime,
                EndBreakTime = workShift.EndBreakTime,
                WorkingTime = workingTime,
                BreakTime = breakTime,
                Description = workShift.Description,
                CreatedBy = workShift.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = workShift.CreatedBy,
                ModifiedDate = DateTime.UtcNow,
                IsActive = 1,
                IsDeleted = 0
            };

            // gọi hàm tạo mới 
            return await _workShiftRepo.InsertAsync(workShiftNew);
        }


        /// <summary>
        /// hàm cập nhật thông tin ca làm việc
        /// </summary>
        /// <param name="id">id của ca làm việc cần cập nhật</param>
        /// <param name="workShift">thuộc tính của ca làm việc</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// <exception cref="NotFoundException">lỗi 404 nếu k tìm thấy</exception>
        /// <exception cref="ValidateException">lỗi 400 nếu sai validate</exception>
        /// /// created by: DuyLC(29/11/2025)
        public async Task<int> UpdateAsync(Guid id, WorkShiftUpdateDto workShift)
        {
            // tìm kiếm bản ghi theo id để cập nhật
            var existing = await _workShiftRepo.GetById(id);
            //kiểm tra có tìm thấy bản ghi không hoặc bản ghi đã bị xóa chưa
            if (existing == null || existing.IsDeleted == 1)
            {
                throw new NotFoundException("Không tìm thấy ca làm việc cần cập nhật.");
            }

            // check validate của thông tin nhập vào
            ValidateShiftPayload(workShift.WorkShiftCode, workShift.WorkShiftName, 
                workShift.BeginShiftTime,  workShift.EndShiftTime, workShift.BeginBreakTime, 
                workShift.EndBreakTime);
            // kiểm tra mã ca làm việc có bị trùng hay không(check trừ bản ghi cần cập nhật)
            await EnsureCodeUnique(workShift.WorkShiftCode, id);
            // kiểm tra nhân viên có tồn tại
            await EnsureEmployeeExists(workShift.ModifiedBy);

            // tính lại số giờ của ca làm và số giờ nghỉ
            var (workingHours, breakHours) = CalculateHours(workShift.BeginShiftTime,
                workShift.EndShiftTime, workShift.BeginBreakTime, workShift.EndBreakTime);

            //set lại giá trị của ca làm việc
            existing.WorkShiftCode = workShift.WorkShiftCode.Trim();
            existing.WorkShiftCode = workShift.WorkShiftCode.Trim();
            existing.BeginShiftTime = workShift.BeginShiftTime;
            existing.EndShiftTime = workShift.EndShiftTime;
            existing.BeginBreakTime = workShift.BeginBreakTime;
            existing.EndBreakTime = workShift.EndBreakTime;
            existing.Description = workShift.Description;
            existing.WorkingTime = workingHours;
            existing.BreakTime = breakHours;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = workShift.ModifiedBy;
            //
            // gọi hàm update
            return await _workShiftRepo.UpdateAsync(id, existing);
        }

        /// <summary>
        /// hàm xóa ca làm việc
        /// </summary>
        /// <param name="id">id của ca làm việc cần xóa</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// <exception cref="NotFoundException">lỗi 404 nếu k tìm thấy</exception>
        /// /// created by: DuyLC(29/11/2025)
        public async Task<int> DeleteAsync(Guid id)
        {
            var existing = await _workShiftRepo.GetById(id);
            if (existing == null || existing.IsDeleted == 1)
            {
                throw new NotFoundException("Không tìm thấy ca làm việc.");
            }

            return await _workShiftRepo.DeleteAsync(id);
        }

        /// <summary>
        /// hàm xóa một list đc chọn ca làm việc
        /// </summary>
        /// <param name="ids">list id của ca làm việc cần xóa</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// <exception cref="NotFoundException">lỗi 404 nếu k tìm thấy</exception>
        /// /// created by: DuyLC(29/11/2025)
        public Task<int> DeleteManyAsync(IEnumerable<Guid> ids)
        {
            return _workShiftRepo.DeleteManyAsync(ids);
        }
        /// <summary>
        /// hàm cập nhật lại trạng thái của ca làm việc (sử dụng, ngưng sử dụng)
        /// </summary>
        /// <param name="workShiftBulkStatus">có thể là một id hoặc một list id cần đc thay đổi
        /// status thành sử dụng hoặc ngưng sử dụng</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// <exception cref="ValidateException">lỗi 404 nếu k truyền object về</exception>
        /// /// created by: DuyLC(29/11/2025)
        public Task<int> UpdateStatusAsync(WorkShiftBulkStatusDto workShiftBulkStatus)
        {
            if (workShiftBulkStatus.Ids == null || !workShiftBulkStatus.Ids.Any())
            {
                throw new ValidateException("Không có ca làm việc nào được chọn.");
            }

            return _workShiftRepo.UpdateStatusAsync(workShiftBulkStatus.Ids, workShiftBulkStatus.IsActive, workShiftBulkStatus.ModifiedBy);
        }
        /// <summary>
        /// hàm cập nhật lại trạng thái của ca làm việc (sử dụng, ngưng sử dụng)
        /// </summary>
        /// <param name="workShiftBulkStatus">có thể là một id hoặc một list id cần đc thay đổi
        /// status thành sử dụng hoặc ngưng sử dụng</param>
        /// <returns>trả vế số dòng bị ảnh hưởng</returns>
        /// <exception cref="ValidateException">lỗi 404 nếu k truyền object về</exception>
        /// /// created by: DuyLC(29/11/2025)
        public Task<IEnumerable<WorkShiftListItemDto>> GetExportDataAsync(WorkShiftFilterDtoRequest request)
        {
            return _workShiftRepo.GetExportDataAsync(request);
        }

        /// <summary>
        /// hàm kiểm tra mã ca làm đã tồn tại hay chưa
        /// </summary>
        /// <param name="workShiftCode">giá trị mã ca làm cần kiểm tra</param>
        /// <param name="excludeId">ID của ca làm việc hiện tại (null khi tạo mới, 
        /// có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>trả vế true/false</returns>
        /// <exception cref="ValidateException">lỗi 404 nếu k truyền object về</exception>
        /// /// created by: DuyLC(29/11/2025)
        public Task<bool> CheckCodeExistsAsync(string workShiftCode, Guid? excludeId)
        {
            return _workShiftRepo.CheckCodeExistsAsync(workShiftCode, excludeId);
        }

        #region Helpers

        /// <summary>
        /// Validate đầy đủ payload tạo/cập nhật ca làm việc trước khi xử lý business logic
        /// </summary>
        /// <param name="workShiftCode">Mã ca làm việc (max 20 ký tự)</param>
        /// <param name="workShiftName">Tên ca làm việc (max 50 ký tự)</param>
        /// <param name="beginShiftTime">Thời gian bắt đầu ca làm việc</param>
        /// <param name="endShiftTime">Thời gian kết thúc ca làm việc</param>
        /// <param name="beginBreakTime">Thời gian bắt đầu nghỉ giữa ca (nullable)</param>
        /// <param name="endBreakTime">Thời gian kết thúc nghỉ giữa ca (nullable)</param>
        /// <exception cref="ValidateException">Ném lỗi 400 
        /// cho các trường hợp validate thất bại</exception>
        /// created by: DuyLC (29/11/2025)
        private static void ValidateShiftPayload(string workShiftCode, string workShiftName, 
            TimeSpan beginShiftTime, TimeSpan endShiftTime, TimeSpan? beginBreakTime, TimeSpan? endBreakTime)
        {
            if (string.IsNullOrWhiteSpace(workShiftCode))
            {
                throw new ValidateException("Mã ca không được để trống.");
            }

            if (workShiftName.Length > 20)
            {
                throw new ValidateException("Mã ca không được vượt quá 20 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(workShiftName))
            {
                throw new ValidateException("Tên ca không được để trống.");
            }

            if (workShiftName.Length > 50)
            {
                throw new ValidateException("Tên ca không được vượt quá 50 ký tự.");
            }

            if (beginShiftTime == null)
            {
                throw new ValidateException("Giờ vào ca không được để trống.");
            }

            if (endShiftTime == null)
            {
                throw new ValidateException("Giờ hết ca không được để trống.");
            }

            if (endShiftTime == beginShiftTime)
            {
                throw new ValidateException("Giờ hết ca không được bằng giờ vào ca.");
            }

            if (beginBreakTime.HasValue && !endBreakTime.HasValue)
            {
                throw new ValidateException("Bạn đã nhập thời gian bắt đầu nghỉ giữa ca nhưng " +
                    "chưa nhập thời gian kết thúc nghỉ giữa ca. Vui lòng kiểm tra lại.");
            }

            if (!beginBreakTime.HasValue && endBreakTime.HasValue)
            {
                throw new ValidateException("Bạn đã nhập thời gian kết thúc nghỉ giữa ca nhưng " +
                    "chưa nhập thời gian bắt đầu nghỉ giữa ca. Vui lòng kiểm tra lại.");
            }

            if (beginBreakTime.HasValue && endBreakTime.HasValue)
            {
                if (endBreakTime.Value <= beginBreakTime.Value)
                {
                    throw new ValidateException("Thời gian nghỉ giữa ca không hợp lệ.");
                }

                if (beginBreakTime.Value < beginShiftTime || endBreakTime.Value > endShiftTime)
                {
                    throw new ValidateException("Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời " +
                        "gian tính từ giờ vào ca đến giờ hết ca. Vui lòng kiểm tra lại.");
                }
            }
        }


        /// <summary>
        /// hàm tính số giờ của ca làm và số giờ nghỉ
        /// </summary>
        /// <param name="beginShiftTime">giờ vào ca</param>
        /// <param name="endShiftTime">giờ hết ca</param>
        /// <param name="beginBreakTime">bắt đầu nghỉ giữa ca làm</param>
        /// <param name="endBreakTime">Kết thúc nghỉ giữa ca làm</param>
        /// <returns>workingTime: Số giờ làm việc</returns>
        /// <returns>breakTime: Số giờ nghỉ giữa ca</returns>
        /// <exception cref="NotFoundException">lỗi 404 nếu k tìm thấy</exception>
        /// <exception cref="ValidateException">lỗi 400 nếu sai validate</exception>
        /// /// created by: DuyLC(29/11/2025)
        private static (decimal workingTime, decimal breakTime) CalculateHours(TimeSpan beginShiftTime, TimeSpan endShiftTime, TimeSpan? beginBreakTime, TimeSpan? endBreakTime)
        {

            // tổng số giờ của ca làm
            var totalHours = (decimal)beginShiftTime.TotalHours;
            if (beginShiftTime <= endShiftTime && endShiftTime != TimeSpan.Zero)
            {
                // tính số giờ nếu bắt đầu làm từ ngày hôm nay tới ngày hôm sau 
                totalHours = (decimal)((24 - beginShiftTime.TotalHours) + endShiftTime.TotalHours);
            }
            else
            {
                totalHours = (decimal)(endShiftTime - beginShiftTime).TotalHours;
            }
           
            // số giờ nghỉ giữa ca trả về
            decimal breakTime = 0;

            //bắt buộc khi nhập thời gian nghỉ thì sẽ phải nhập cả thời gian bắt đầu và kết thúc
            if (beginBreakTime.HasValue && endBreakTime.HasValue)
            {
                // tính số giờ nghỉ giũa ca
                breakTime = (decimal)(endBreakTime.Value - beginBreakTime.Value).TotalHours;
            }
            // số thời gian làm việc trong ca làm
            var workingTime = totalHours - breakTime;

            // kiểm tra số giờ làm việc không âm
            if (workingTime <= 0)
            {
                throw new ValidateException("Thời gian làm việc phải lớn hơn 0.");
            }

            return (Math.Round(workingTime, 2), Math.Round(breakTime, 2));
        }


        /// <summary>
        /// Hàm kiểm tra tính duy nhất của mã ca làm việc trước khi tạo mới hoặc cập nhật
        /// </summary>
        /// <param name="workShiftCode">Mã ca làm việc cần kiểm tra</param>
        /// <param name="excludeId">ID của ca làm việc hiện tại (null khi tạo mới, 
        /// có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <exception cref="ConflictException">Ném lỗi 409 Conflict 
        /// nếu mã ca đã tồn tại trong database</exception>
        /// created by: DuyLC (29/11/2025)
        private async Task EnsureCodeUnique(string workShiftCode, Guid? excludeId)
        {
            var existed = await _workShiftRepo.CheckCodeExistsAsync(workShiftCode.Trim(), excludeId);
            if (existed)
            {
                throw new ConflictException("Mã ca đã tồn tại.");
            }
        }


        /// <summary>
        /// Kiểm tra sự tồn tại của nhân viên theo ID trước khi thực hiện nghiệp vụ
        /// </summary>
        /// <param name="employeeId">ID của nhân viên cần kiểm tra</param>
        /// <exception cref="ValidateException">Ném lỗi 400
        /// nếu không tìm thấy nhân viên</exception>
        /// created by: DuyLC (29/11/2025)
        private async Task EnsureEmployeeExists(Guid employeeId)
        {
            var employee = await _employeeRepo.GetById(employeeId);
            if (employee == null)
            {
                throw new ValidateException("Không tìm thấy thông tin người tạo.");
            }
        }

        #endregion
    }
}

