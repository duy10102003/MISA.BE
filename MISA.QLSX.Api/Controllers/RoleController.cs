using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLSX.Core.Interfaces.Services;

namespace MISA.QLSX.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        /// <summary>
        /// lấy danh sách vai trò 
        /// </summary>
        /// <returns>danh sách vai trò</returns>
        /// Created: DuyLC(29/11/2025)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _roleService.GetAllAsync();
            return StatusCode(200, result); 
        }
    }
}
