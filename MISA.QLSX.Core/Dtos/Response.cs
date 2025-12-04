using MISA.QLSX.Core.Constants;
using MISA.QLSX.Core.Enums;

namespace MISA.QLSX.Core.Dtos
{
    /// <summary>
    /// Class Response chuẩn cho tất cả API
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của data</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Mã code trả về (sử dụng ResponseCode enum)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        public Response(int code, T? data, string message)
        {
            Code = code;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static Response<T> Success(T? data, string message = "")
        {
            return new Response<T>
            {
                Code = (int)ResponseCode.Success,
                Data = data,
                Message = string.IsNullOrEmpty(message) ? ResponseMessage.Success : message
            };
        }

        /// <summary>
        /// Tạo response lỗi
        /// </summary>
        public static Response<T> Error(int code, string message, T? data = default)
        {
            return new Response<T>
            {
                Code = code,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Tạo response lỗi với ResponseCode
        /// </summary>
        public static Response<T> Error(ResponseCode code, string message, T? data = default)
        {
            return new Response<T>
            {
                Code = (int)code,
                Data = data,
                Message = message
            };
        }
    }

    /// <summary>
    /// Response không có data
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Mã code trả về
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        public Response(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static Response Success(string message = "")
        {
            return new Response
            {
                Code = (int)ResponseCode.Success,
                Message = string.IsNullOrEmpty(message) ? ResponseMessage.Success : message
            };
        }

        /// <summary>
        /// Tạo response lỗi
        /// </summary>
        public static Response Error(int code, string message)
        {
            return new Response
            {
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// Tạo response lỗi với ResponseCode
        /// </summary>
        public static Response Error(ResponseCode code, string message)
        {
            return new Response
            {
                Code = (int)code,
                Message = message
            };
        }
    }
}

