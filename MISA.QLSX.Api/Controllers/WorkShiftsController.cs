using Microsoft.AspNetCore.Mvc;
using MISA.QLSX.Core.Constants;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Enums;
using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Interfaces.Services;
using System.Globalization;
using System.Text;

namespace MISA.QLSX.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkShiftsController : ControllerBase
    {
        private readonly IWorkShiftService _workShiftService;

        public WorkShiftsController(IWorkShiftService workShiftService)
        {
            _workShiftService = workShiftService;
        }

        /// <summary>
        /// Lấy danh sách ca làm việc có phân trang và lọc
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin phân trang, tìm kiếm và sắp xếp</param>
        /// <returns>Danh sách ca làm việc có phân trang</returns>
        /// Created: Auto
        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll([FromBody] WorkShiftFilterDtoRequest request)
        {
            try
            {
                var result = await _workShiftService.GetPagingAsync(request);
                var response = Response<PagedResult<WorkShiftListItemDto>>.Success(result, ResponseMessage.Success);
                return Ok(response);
            }
            catch (ValidateException ex)
            {
                var response = Response<PagedResult<WorkShiftListItemDto>>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = Response<PagedResult<WorkShiftListItemDto>>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết ca làm việc theo ID
        /// </summary>
        /// <param name="id">ID của ca làm việc cần lấy</param>
        /// <returns>Thông tin chi tiết ca làm việc</returns>
        /// Created: Auto
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var result = await _workShiftService.GetByIdAsync(id);
                var response = Response<WorkShiftDetailDto>.Success(result, ResponseMessage.Success);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = Response<WorkShiftDetailDto>.Error(
                    ResponseCode.NotFound,
                    ex.UserMsg ?? ResponseMessage.NotFound
                );
                return NotFound(response);
            }
            catch (Exception)
            {
                var response = Response<WorkShiftDetailDto>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Kiểm tra mã ca làm việc đã tồn tại chưa
        /// </summary>
        /// <param name="code">Mã ca làm việc cần kiểm tra</param>
        /// <param name="excludeId">ID ca làm việc cần loại trừ (dùng khi cập nhật)</param>
        /// <returns>True nếu mã đã tồn tại, False nếu chưa tồn tại</returns>
        /// Created: Auto
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCode([FromQuery] string code, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                var isExisted = await _workShiftService.CheckCodeExistsAsync(code, excludeId);
                var message = isExisted ? ResponseMessage.CodeExists : ResponseMessage.CodeNotExists;
                var response = Response<bool>.Success(isExisted, message);
                return Ok(response);
            }
            catch (Exception)
            {
                var response = Response<bool>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Tạo mới ca làm việc
        /// </summary>
        /// <param name="request">Thông tin ca làm việc cần tạo</param>
        /// <returns>ID của ca làm việc vừa tạo (Status 201)</returns>
        /// Created: Auto
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkShift([FromBody] WorkShiftCreateDto request)
        {
            try
            {
                var id = await _workShiftService.CreateAsync(request);
                var response = Response<Guid>.Success(id, ResponseMessage.Created);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ValidateException ex)
            {
                var response = Response<Guid>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (ConflictException ex)
            {
                var response = Response<Guid>.Error(
                    ResponseCode.Conflict,
                    ex.UserMsg ?? ResponseMessage.CodeExists
                );
                return Conflict(response);
            }
            catch (Exception)
            {
                var response = Response<Guid>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Cập nhật thông tin ca làm việc
        /// </summary>
        /// <param name="id">ID của ca làm việc cần cập nhật</param>
        /// <param name="request">Thông tin ca làm việc cần cập nhật</param>
        /// <returns>Số lượng bản ghi đã cập nhật</returns>
        /// Created: Auto
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkShift(Guid id, [FromBody] WorkShiftUpdateDto request)
        {
            try
            {
                var result = await _workShiftService.UpdateAsync(id, request);
                var response = Response<int>.Success(result, ResponseMessage.Updated);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.NotFound,
                    ex.UserMsg ?? ResponseMessage.NotFound
                );
                return NotFound(response);
            }
            catch (ValidateException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (ConflictException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.Conflict,
                    ex.UserMsg ?? ResponseMessage.CodeExists
                );
                return Conflict(response);
            }
            catch (Exception)
            {
                var response = Response<int>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Xóa một ca làm việc theo ID
        /// </summary>
        /// <param name="id">ID của ca làm việc cần xóa</param>
        /// <returns>Số lượng bản ghi đã xóa</returns>
        /// Created: Auto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _workShiftService.DeleteAsync(id);
                var response = Response<int>.Success(result, ResponseMessage.Deleted);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.NotFound,
                    ex.UserMsg ?? ResponseMessage.NotFound
                );
                return NotFound(response);
            }
            catch (Exception)
            {
                var response = Response<int>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Xóa nhiều ca làm việc cùng lúc
        /// </summary>
        /// <param name="request">Đối tượng chứa danh sách ID các ca làm việc cần xóa</param>
        /// <returns>Số lượng bản ghi đã xóa</returns>
        /// Created: Auto
        [HttpDelete("multiple-delete")]
        public async Task<IActionResult> DeleteMany([FromBody] WorkShiftDeleteManyDto request)
        {
            try
            {
                var result = await _workShiftService.DeleteManyAsync(request.Ids);
                var response = Response<int>.Success(result, ResponseMessage.DeletedMany);
                return Ok(response);
            }
            catch (ValidateException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = Response<int>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Cập nhật trạng thái (sử dụng/ngừng sử dụng) cho nhiều ca làm việc
        /// </summary>
        /// <param name="request">Đối tượng chứa danh sách ID và trạng thái mới</param>
        /// <returns>Số lượng bản ghi đã cập nhật</returns>
        /// Created: Auto
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] WorkShiftBulkStatusDto request)
        {
            try
            {
                var result = await _workShiftService.UpdateStatusAsync(request);
                var response = Response<int>.Success(result, ResponseMessage.StatusUpdated);
                return Ok(response);
            }
            catch (ValidateException ex)
            {
                var response = Response<int>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = Response<int>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Xuất danh sách ca làm việc ra file CSV
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin lọc để xuất dữ liệu</param>
        /// <returns>File CSV chứa danh sách ca làm việc</returns>
        /// Created: Auto
        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] WorkShiftFilterDtoRequest request)
        {
            try
            {
                var data = await _workShiftService.GetExportDataAsync(request);

                var csv = BuildCsv(data);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var fileName = $"work-shifts-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
                return File(bytes, "text/csv", fileName);
            }
            catch (ValidateException ex)
            {
                var response = Response<object>.Error(
                    ResponseCode.BadRequest,
                    ex.UserMsg ?? ResponseMessage.InvalidData
                );
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = Response<object>.Error(
                    ResponseCode.InternalServerError,
                    ResponseMessage.InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            finally
            {
                // Cleanup nếu cần
            }
        }

        /// <summary>
        /// Xây dựng chuỗi CSV từ danh sách ca làm việc
        /// </summary>
        /// <param name="data">Danh sách ca làm việc cần xuất</param>
        /// <returns>Chuỗi CSV</returns>
        private static string BuildCsv(IEnumerable<WorkShiftListItemDto> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Ma ca,Ten ca,Gio vao ca,Gio ket thuc,Bat dau nghi,Gio ket thuc nghi,Thoi gian lam viec,Thoi gian nghi,Trang thai,Nguoi tao,Ngay tao,Nguoi sua,Ngay sua");

            foreach (var item in data)
            {
                var line = string.Join(",",
                    Escape(item.WorkShiftCode),
                    Escape(item.WorkShiftName),
                    item.BeginShiftTime.ToString(@"hh\:mm"),
                    item.EndShiftTime.ToString(@"hh\:mm"),
                    item.BeginBreakTime?.ToString(@"hh\:mm") ?? string.Empty,
                    item.EndBreakTime?.ToString(@"hh\:mm") ?? string.Empty,
                    item.WorkingTime.ToString(CultureInfo.InvariantCulture),
                    item.BreakTime.ToString(CultureInfo.InvariantCulture),
                    item.IsActive == 1 ? "Su dung" : "Ngung su dung",
                    Escape(item.CreatedByName ?? string.Empty),
                    item.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    Escape(item.ModifiedByName ?? string.Empty),
                    item.ModifiedDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty);

                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escape các ký tự đặc biệt trong CSV (dấu phẩy, dấu ngoặc kép)
        /// </summary>
        /// <param name="value">Giá trị cần escape</param>
        /// <returns>Giá trị đã được escape</returns>
        private static string Escape(string value)
        {
            if (value.Contains(",") || value.Contains("\""))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}

