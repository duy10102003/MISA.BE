using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Interfaces.Services;

namespace MISA.QLSX.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
       
        /// <summary>
        /// lấy chi tiết nhân viên theo id
        /// </summary>
        /// <param name="id">id của nhân viên</param>
        /// <returns>thoogn tin của nhân viên</returns>
        /// Created: TuanDQ(17/11/2025)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetById(id);

            return StatusCode(200, employee);
        }
    }
}
