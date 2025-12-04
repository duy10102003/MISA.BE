using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLSX.Core.Constants;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Enums;
using MISA.QLSX.Core.Exceptions;
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
        /// Lấy danh sách tất cả vai trò trong hệ thống
        /// </summary>
        /// <returns>Danh sách vai trò</returns>
        /// Created: DuyLC(29/11/2025)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _roleService.GetAllAsync();
                var response = Response<object>.Success(result, ResponseMessage.Success);
                return Ok(response);
            }
            catch (Exception ex)
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
    }
}
