using Microsoft.AspNetCore.Mvc;
using MISA.QLSX.Core.Dtos;
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

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll([FromBody] WorkShiftFilterDtoRequest request)
        {
            var result = await _workShiftService.GetPagingAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _workShiftService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCode([FromQuery] string code, [FromQuery] Guid? excludeId = null)
        {
            var isExisted = await _workShiftService.CheckCodeExistsAsync(code, excludeId);
            return Ok(isExisted);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkShift([FromBody] WorkShiftCreateDto request)
        {
            var id = await _workShiftService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkShift(Guid id, [FromBody] WorkShiftUpdateDto request)
        {
            var result = await _workShiftService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _workShiftService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpDelete("multiple-delete")]
        public async Task<IActionResult> DeleteMany([FromBody] WorkShiftDeleteManyDto request)
        {
            var result = await _workShiftService.DeleteManyAsync(request.Ids);
            return Ok(result);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] WorkShiftBulkStatusDto request)
        {
            var result = await _workShiftService.UpdateStatusAsync(request);
            return Ok(result);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] WorkShiftFilterDtoRequest request)
        {
            var data = await _workShiftService.GetExportDataAsync(request);

            var csv = BuildCsv(data);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var fileName = $"work-shifts-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

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

