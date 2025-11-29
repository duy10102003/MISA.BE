using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Interfaces.Repositories;
using MISA.QLSX.Core.MISAAttribute;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MISA.QLSX.Infrastructure.Repositories
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        private readonly string _connection;
        private readonly string _tableName; // lấy ra tên bảng
        private readonly PropertyInfo _keyAtribute; // lấy ra khóa chính của bảng
        private readonly string _keyColumn; // lấy ra tên các cột trong bảng

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="configuration"></param>
        public BaseRepo(IConfiguration configuration)
        {
            // lấy ra chuỗi connection
            _connection = configuration.GetConnectionString("DefaultConnection");
            // gọi hàm lấy tên bảng
            _tableName = getTableName();
            // lấy properties của kiểu T
            var properties = typeof(T).GetProperties();
            _keyAtribute = properties.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));

            if (_keyAtribute == null)
            {
                throw new ValidateException($"Entity {typeof(T).Name} không có thuộc tính nào được đánh dấu [MISAKey]");
            }
            // gọi hàm lấy tên cột 
            _keyColumn = getColumnName(_keyAtribute);

        }

        /// <summary>
        /// hàm lấy ra tên của cột trong bảng
        /// </summary>
        /// <param name="property">thông tin của cột</param>
        /// <returns>tên cột</returns>
        /// created by: DuyLC (29/11/2025)
        public string getColumnName(PropertyInfo property)
        {
            var columnAttribute = (ColumnAttribute?)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
            if (columnAttribute != null)
            {
                return $"`{columnAttribute.Name}`";
            }
            return $"`{property.Name}`";
        }

        /// <summary>
        /// Hàm lấy ra tên của bảng
        /// </summary>
        /// <param name="entity">thực thể mà mình cần lấy tên</param>
        /// <returns>tên của bảng</returns>
        /// created by: DuyLC (29/11/2025)
        public string getTableName()
        {
            // Lấy name của kiểu T thông qua reflection.
            var tableName = typeof(T).Name;
            var tableAttribute = (MISATableNameAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(MISATableNameAttribute));
            if (tableAttribute != null)
            {
                tableName = tableAttribute.TableName;
            }
            return tableName;
        }

        /// <summary>
        /// xóa mềm 1 bản ghi trong database
        /// </summary>
        /// <param name="id">id mà mình muốn xóa </param>
        /// <returns>số dòng bị nahr hưởng </returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<int> DeleteAsync(Guid id)
        {
            //câu lệnh sql xóa mềm theo id
            var sql = $"UPDATE {_tableName} SET is_deleted = 1 WHERE {_keyColumn} = @id;";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.ExecuteAsync(sql, parameter);
                return result;
            }
        }
        /// <summary>
        /// lấy ra tất cả danh sách của thực thể
        /// </summary>
        /// <returns>danh sách thực thể</returns>
        /// created by: DuyLC(29/11/2025)
        public async Task<List<T>> GetAllAsync()
        {
            // câu lệnh sql lấy tất cả danh sách
            var sql = $"select * from {_tableName} where is_deleted = 0";
            // kết nối đến database bằng dapper và npgsqlconnection
            using (var connection = new MySqlConnection(_connection))
            {
                var result = await connection.QueryAsync<T>(sql);
                return result.ToList();
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết theo id
        /// </summary>
        /// <param name="id"> id của đối tượng mà mình muốn tìm </param>
        /// <returns>đối tượng có id phù hợp</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<T?> GetById(Guid id)
        {
            //câu lệnh sql lấy chi tiết theo id
            var sql = $"select * from {_tableName} where  {_tableName}_id = @id and is_deleted = 0";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            Console.WriteLine(sql);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.QueryAsync<T>(sql, parameter);
                return result.FirstOrDefault();
            }
        }
        /// <summary>
        /// hàm insert 
        /// </summary>
        /// <param name="entity">thông tin đối tượng cần insert</param>
        /// <returns>id của đối tượng vừa thêm</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<Guid> InsertAsync(T entity)
        {
            
            var props = typeof(T).GetProperties();
            // lấy ra danh sách các cột cách nhau bằng dấu , cho câu lệnh insert
            var columns = string.Join(", ", props.Select(p => getColumnName(p)));
            // lấy ra danh sách param cần truyền vào dựa vào tên các cột đã lấy
            var param = string.Join(", ", props.Select(p => $"@{p.Name}"));
            //tạo new Guid mới
            var key = _keyAtribute;
            // kiểm tra nếu key là Guid mới tạo ra id mới 
            if (key.PropertyType == typeof(Guid) && (Guid)key.GetValue(entity)! == Guid.Empty)
            {
                key.SetValue(entity, Guid.NewGuid());
            }
            //câu lệnh sql
            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({param})";
            // thực hiện kết nối 
            using (var connection = new MySqlConnection(_connection))
            {
                // thực thi câu lệnh
                await connection.ExecuteAsync(sql, param: entity);
                return (Guid)key.GetValue(entity)!;
            }
        }
        /// <summary>
        /// Hàm cập nhật thông tin
        /// </summary>
        /// <param name="id">id của đối tượng cần cật nhập</param>
        /// <param name="entity">thông tin cập nhật</param>
        /// <returns>số dòng bị ảnh hưởng</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<int> UpdateAsync(Guid id, T entity)
        {
            var props = typeof(T).GetProperties()
                           .Where(p => p != _keyAtribute);

            var setClause = string.Join(", ",
                props.Select(p => $"{getColumnName(p)} = @{p.Name}"));

            var sql = $"UPDATE {_tableName} SET {setClause} WHERE {_keyColumn} = @id";

            var parameters = new DynamicParameters(entity);
            parameters.Add("@id", id);

            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
