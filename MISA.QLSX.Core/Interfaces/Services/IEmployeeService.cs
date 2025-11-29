using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<Employee> GetById(Guid id);

    }
}
