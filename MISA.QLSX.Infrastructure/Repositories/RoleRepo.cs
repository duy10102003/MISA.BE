using Microsoft.Extensions.Configuration;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Infrastructure.Repositories
{
    public class RoleRepo : BaseRepo<Role>, IRoleRepo
    {
        public RoleRepo(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
