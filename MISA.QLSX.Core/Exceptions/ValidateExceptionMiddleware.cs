using Microsoft.AspNetCore.Http;
using MISA.QLSX.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Exceptions
{
    /// <summary>
    /// Custom middleware xử lý Validate exception
    /// Created by: DuyLC(29/11/2025)
    /// </summary>
    public class ValidateExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ValidateExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidateException ex)
            {
                await HandleExceptionAsync(context, 400, "Dữ liệu không hợp lệ", ex);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, 404, "Không tìm thấy dữ liệu", ex);
            }
            catch (ConflictException ex)
            {
                await HandleExceptionAsync(context, 409, "Dữ liệu bị trùng", ex);
            }
            catch (System.Exception ex)
            {
                await HandleExceptionAsync(context, 500, "Lỗi hệ thống", ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string defaultUserMsg, Exception ex)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var baseEx = ex as BaseException;

            var res = new
            {
                ErrorCode = $"ERR_{statusCode}",
                DevMsg = ex.Message, // Chi tiết cho dev (để log lại)
                UserMsg = baseEx?.UserMsg ?? defaultUserMsg, // Hiển thị ra user
                TraceId = context.TraceIdentifier
            };

            var resJson = System.Text.Json.JsonSerializer.Serialize(res);
            await context.Response.WriteAsync(resJson);
        }
    }
}
