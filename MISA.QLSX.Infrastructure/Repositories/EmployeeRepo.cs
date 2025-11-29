using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Interfaces.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLSX.Infrastructure.Repositories
{
    public class EmployeeRepo : BaseRepo<Employee>, IEmployeeRepo
    {
        private readonly string _connection;
        public EmployeeRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

    }
}
