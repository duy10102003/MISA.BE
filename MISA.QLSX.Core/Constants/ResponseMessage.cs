namespace MISA.QLSX.Core.Constants
{
    /// <summary>
    /// Constants chứa các message trả về cho API
    /// </summary>
    public static class ResponseMessage
    {
        /// <summary>
        /// Thành công
        /// </summary>
        public const string Success = "Thành công";

        /// <summary>
        /// Tạo mới thành công
        /// </summary>
        public const string Created = "Tạo mới thành công";

        /// <summary>
        /// Cập nhật thành công
        /// </summary>
        public const string Updated = "Cập nhật thành công";

        /// <summary>
        /// Xóa thành công
        /// </summary>
        public const string Deleted = "Xóa thành công";

        /// <summary>
        /// Xóa nhiều thành công
        /// </summary>
        public const string DeletedMany = "Xóa nhiều bản ghi thành công";

        /// <summary>
        /// Cập nhật trạng thái thành công
        /// </summary>
        public const string StatusUpdated = "Cập nhật trạng thái thành công";

        /// <summary>
        /// Xuất dữ liệu thành công
        /// </summary>
        public const string Exported = "Xuất dữ liệu thành công";

        /// <summary>
        /// Dữ liệu không hợp lệ
        /// </summary>
        public const string InvalidData = "Dữ liệu không hợp lệ";

        /// <summary>
        /// Không tìm thấy dữ liệu
        /// </summary>
        public const string NotFound = "Không tìm thấy dữ liệu";

        /// <summary>
        /// Mã đã tồn tại
        /// </summary>
        public const string CodeExists = "Mã đã tồn tại trong hệ thống";

        /// <summary>
        /// Mã chưa tồn tại
        /// </summary>
        public const string CodeNotExists = "Mã chưa tồn tại trong hệ thống";

        /// <summary>
        /// Lỗi server
        /// </summary>
        public const string InternalServerError = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";

        /// <summary>
        /// Lỗi không xác định
        /// </summary>
        public const string UnknownError = "Đã xảy ra lỗi không xác định";
    }
}

