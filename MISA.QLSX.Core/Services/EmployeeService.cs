using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Interfaces.Repositories;
using MISA.QLSX.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepo _employeeRepo;

        public EmployeeService(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        /// <summary>
        /// lấy nhân viên lọc theo Id
        /// </summary>
        /// <param name="id">id nhân viên cần tìm</param>
        /// <returns>nhân viên hợp lệ</returns>
        /// <exception cref="NotFoundException">trả về lỗi 404 not found nếu không có data</exception>
        /// created by: DuyLC(29/11/2025)
        public Task<Employee> GetById(Guid id)
        {
            var result = _employeeRepo.GetById(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy giá trị phù hợp.");
            }
            return result;
        }
        
        
    }
}
