namespace MISA.QLSX.Core.Enums
{
    /// <summary>
    /// Enum định nghĩa các mã code trả về cho API
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 200,

        /// <summary>
        /// Tạo mới thành công
        /// </summary>
        Created = 201,

        /// <summary>
        /// Không có nội dung
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// Dữ liệu không hợp lệ
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Không được phép truy cập
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Không tìm thấy
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Xung đột dữ liệu (vd: mã đã tồn tại)
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// Lỗi server
        /// </summary>
        InternalServerError = 500
    }
}

