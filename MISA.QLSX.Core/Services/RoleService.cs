using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Interfaces.Repositories;
using MISA.QLSX.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepo _roleRepo;
        public RoleService(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }
        public Task<List<Role>> GetAllAsync()
        {
            var result = _roleRepo.GetAllAsync();
            if (result == null)
            {
                throw new NotFoundException("Không có danh sách giá trị phù hợp");
            }
            return result;
        }
    }
}
