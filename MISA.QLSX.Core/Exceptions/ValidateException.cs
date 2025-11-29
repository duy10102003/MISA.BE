namespace MISA.QLSX.Core.Exceptions
{
    /// <summary>
    /// 400 Dữ liệu không hợp lệ
    /// Created by: DuyLC(29/11/2025)
    /// </summary> 
    public class ValidateException : BaseException
    {
        public ValidateException(string devMsg, string? userMsg = null)
            : base(devMsg, userMsg ?? "Dữ liệu bạn nhập chưa hợp lệ.") { }
    }
}
