using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLSX.Core.Dtos;
using MISA.QLSX.Core.Entities;
using MISA.QLSX.Core.Interfaces.Repositories;
using MySqlConnector;
using Npgsql;
using System.Linq;
using System.Text;

namespace MISA.QLSX.Infrastructure.Repositories
{
    public class WorkShiftRepo : BaseRepo<WorkShift>, IWorkShiftRepo
    {
        private readonly string _connection;


        public WorkShiftRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResult<WorkShiftListItemDto>> GetPagingAsync(WorkShiftFilterDtoRequest workShiftFilter)
        {
            var page = workShiftFilter.Page <= 0 ? 1 : workShiftFilter.Page;
            var pageSize = workShiftFilter.PageSize <= 0 ? 10 : workShiftFilter.PageSize;
            var offset = (page - 1) * pageSize;

            var whereBuilder = new StringBuilder("WHERE ws.is_deleted = 0");
            var orderBuilder = new StringBuilder();
            var parameters = new DynamicParameters();

            // Giữ lại keyword để tương thích
            if (!string.IsNullOrWhiteSpace(workShiftFilter.Keyword))
            {
                whereBuilder.Append(" AND (LOWER(ws.shift_code) LIKE @keyword OR LOWER(ws.shift_name) LIKE @keyword)");
                parameters.Add("@keyword", $"%{workShiftFilter.Keyword.Trim().ToLower()}%");
            }

            // Xử lý column filters
            if (workShiftFilter.ColumnFilters != null && workShiftFilter.ColumnFilters.Any())
            {
                ApplyColumnFilters(whereBuilder, parameters, workShiftFilter.ColumnFilters);
            }

            // xử lý column sorts
            if (workShiftFilter.ColumnSorts!= null && workShiftFilter.ColumnSorts.Any())
            {
                ApplyColumnSorting(orderBuilder, workShiftFilter.ColumnSorts);
            }
            else
            {
                ApplyColumnSorting(orderBuilder, new List<ColumnSortDto>());
            }

            var whereClause = whereBuilder.ToString();
            var orderClause = orderBuilder.ToString();

            var countSql = $@"SELECT COUNT(*) 
                              FROM work_shift ws 
                              {whereClause}";

            var dataSql = $@"SELECT 
                                ws.work_shift_id,
                                ws.work_shift_code,
                                ws.work_shift_name,
                                ws.begin_shift_time,
                                ws.end_shift_time,
                                ws.begin_break_time,
                                ws.end_break_time,
                                ws.working_time,
                                ws.break_time,
                                ws.is_active,
                                ws.description,
                                ws.created_date,
                                ws.modified_date,
                                c.employee_full_name AS CreatedByName,
                                m.employee_full_name AS ModifiedByName
                            FROM work_shift ws
                            LEFT JOIN employee c ON ws.created_by = c.employee_id
                            LEFT JOIN employee m ON ws.modified_by = m.employee_id
                            {whereClause}
                            {orderClause}
                            LIMIT @limit OFFSET @offset";

            parameters.Add("@limit", pageSize);
            parameters.Add("@offset", offset);

            using var connection = new MySqlConnection(_connection);
            var total = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<WorkShiftListItemDto>(dataSql, parameters);

            return new PagedResult<WorkShiftListItemDto>
            {
                Items = items.ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<WorkShiftDetailDto?> GetDetailAsync(Guid id)
        {
            var sql = @"SELECT 
                            ws.work_shift_id,
                            ws.work_shift_code,
                            ws.work_shift_name,
                            ws.begin_shift_time,
                            ws.end_shift_time,
                            ws.begin_break_time,
                            ws.end_break_time,
                            ws.working_time,
                            ws.break_time,
                            ws.is_active,
                            ws.description,
                            ws.created_by,
                            ws.modified_by,
                            ws.created_date,
                            ws.modified_date
                        FROM work_shift ws
                        WHERE ws.work_shift_id = @id AND ws.is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<WorkShiftDetailDto>(sql, new { id });
        }

        public async Task<bool> CheckCodeExistsAsync(string code, Guid? excludeId = null)
        {
            var sql = @"SELECT EXISTS (
                            SELECT 1 
                            FROM work_shift 
                            WHERE LOWER(work_shift_code) = LOWER(@code)
                              AND is_deleted = 0
                              AND (@excludeId IS NULL OR work_shift_id <> @excludeId)
                        ) AS exists_result";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { code, excludeId });
            return result == 1;
        }

        public async Task<int> UpdateStatusAsync(IEnumerable<Guid> ids, int isActive, Guid modifiedBy)
        {

            if (ids == null || !ids.Any())
                return 0;

            // Build chuỗi danh sách id: 'id1','id2','id3'
            var formattedIds = string.Join(",", ids.Select(id => $"'{id}'"));
            var sql = $@"UPDATE work_shift 
                        SET is_active = @isActive,
                            modified_by = @modifiedBy,
                            modified_date = NOW()
                        WHERE work_shift_id IN ({formattedIds})";

            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new
            {
                ids = ids.ToArray(),
                isActive,
                modifiedBy
            });
        }

        public async Task<int> DeleteManyAsync(IEnumerable<Guid> ids)
        {

            if (ids == null || !ids.Any())
                return 0;

            // Build chuỗi danh sách id: 'id1','id2','id3'
            var formattedIds = string.Join(",", ids.Select(id => $"'{id}'"));
            var sql = $@"UPDATE work_shift 
                        SET is_deleted = 1
                        WHERE work_shift_id IN ({formattedIds})";

            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new { ids = ids.ToArray() });
        }

        public async Task<IEnumerable<WorkShiftListItemDto>> GetExportDataAsync(WorkShiftFilterDtoRequest workShiftFilter)
        {
            var whereBuilder = new StringBuilder("WHERE ws.is_deleted = 0");
            var orderBuilder = new StringBuilder();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(workShiftFilter.Keyword))
            {
                whereBuilder.Append(" AND (LOWER(ws.shift_code) LIKE @keyword OR LOWER(ws.shift_name) LIKE @keyword)");
                parameters.Add("@keyword", $"%{workShiftFilter.Keyword.Trim().ToLower()}%");
            }

            // Filters
            if (workShiftFilter.ColumnFilters?.Any() == true)
                ApplyColumnFilters(whereBuilder, parameters, workShiftFilter.ColumnFilters);

            // Sorting cho export
            ApplyColumnSorting(orderBuilder, workShiftFilter.ColumnSorts ?? new List<ColumnSortDto>());


            var whereClause = whereBuilder.ToString();
            var orderClause = orderBuilder.ToString();

            var sql = $@"SELECT 
                                ws.work_shift_id,
                                ws.work_shift_code,
                                ws.work_shift_name,
                                ws.begin_shift_time,
                                ws.end_shift_time,
                                ws.begin_break_time,
                                ws.end_break_time,
                                ws.working_time,
                                ws.break_time,
                                ws.is_active,
                                ws.description,
                                ws.created_date,
                                ws.modified_date,
                                c.employee_full_name AS CreatedByName,
                                m.employee_full_name AS ModifiedByName
                            FROM work_shift ws
                            LEFT JOIN employee c ON ws.created_by = c.employee_id
                            LEFT JOIN employee m ON ws.modified_by = m.employee_id
                            {whereClause}
                            {orderClause} " ;

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryAsync<WorkShiftListItemDto>(sql, parameters);
        }



        #region Helper


        /// <summary>
        /// Áp dụng các column filters vào WHERE clause
        /// </summary>
        private void ApplyColumnFilters(StringBuilder whereBuilder, DynamicParameters parameters, List<ColumnFilterDto> filters)
        {
            var paramIndex = 0;
            var allowedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "work_shift_code",
                "work_shift_name",
                "working_time",
                "break_time",
                "is_active",
                "created_by",
                "modified_by",
                "created_date",
                "modified_date"
            };

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.ColumnName) || string.IsNullOrWhiteSpace(filter.Operator))
                    continue;

                var columnName = filter.ColumnName.ToLower();
                if (!allowedColumns.Contains(columnName))
                    continue;

                var dbColumn = $"ws.{columnName}";
                var paramName = $"@filter{paramIndex}";
                paramIndex++;

                // Xác định loại cột
                var isTextColumn = columnName == "work_shift_code" || columnName == "work_shift_name" || columnName.Contains("by");
                var isTimeColumn = columnName.Contains("_date");
                var isNumericColumn = columnName.Contains("_hours");
                var isBooleanColumn = columnName == "is_active";

                switch (filter.Operator.ToLower())
                {
                    
                    case "contains":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) LIKE {paramName}");
                            parameters.Add(paramName, $"%{filter.Value.Trim().ToLower()}%");
                        }
                        break;

                    case "not_contains":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) NOT LIKE {paramName}");
                            parameters.Add(paramName, $"%{filter.Value.Trim().ToLower()}%");
                        }
                        break;

                    case "equals":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) = {paramName}");
                            parameters.Add(paramName, filter.Value.Trim().ToLower());
                        }
                        else if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn}::text = {paramName}");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} = {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        else if (isBooleanColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (bool.TryParse(filter.Value, out var boolValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} = {paramName}");
                                parameters.Add(paramName, boolValue);
                            }
                        }
                        break;

                    case "not_equals":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) <> {paramName}");
                            parameters.Add(paramName, filter.Value.Trim().ToLower());
                        }
                        else if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn}::text <> {paramName}");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} <> {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        else if (isBooleanColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (bool.TryParse(filter.Value, out var boolValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} <> {paramName}");
                                parameters.Add(paramName, boolValue);
                            }
                        }
                        break;

                    case "starts_with":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) LIKE {paramName}");
                            parameters.Add(paramName, $"{filter.Value.Trim().ToLower()}%");
                        }
                        break;

                    case "ends_with":
                        if (isTextColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND LOWER({dbColumn}) LIKE {paramName}");
                            parameters.Add(paramName, $"%{filter.Value.Trim().ToLower()}");
                        }
                        break;

                    case "empty":
                        if (isTextColumn)
                        {
                            whereBuilder.Append($" AND ({dbColumn} IS NULL OR {dbColumn} = '')");
                        }
                        else if (isTimeColumn)
                        {
                            whereBuilder.Append($" AND {dbColumn} IS NULL");
                        }
                        break;

                    case "not_empty":
                        if (isTextColumn)
                        {
                            whereBuilder.Append($" AND {dbColumn} IS NOT NULL AND {dbColumn} <> ''");
                        }
                        else if (isTimeColumn)
                        {
                            whereBuilder.Append($" AND {dbColumn} IS NOT NULL");
                        }
                        break;

                    
                    case "greater_than":
                        if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn} > {paramName}::time");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} > {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        break;

                    case "less_than":
                        if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn} < {paramName}::time");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} < {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        break;

                    case "greater_or_equal":
                        if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn} >= {paramName}::time");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} >= {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        break;

                    case "less_or_equal":
                        if (isTimeColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            whereBuilder.Append($" AND {dbColumn} <= {paramName}::time");
                            parameters.Add(paramName, filter.Value.Trim());
                        }
                        else if (isNumericColumn && !string.IsNullOrWhiteSpace(filter.Value))
                        {
                            if (decimal.TryParse(filter.Value, out var numValue))
                            {
                                whereBuilder.Append($" AND {dbColumn} <= {paramName}");
                                parameters.Add(paramName, numValue);
                            }
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// Áp dụng dynamic column sorting vào ORDER BY clause
        /// </summary>
        /// <param name="orderBuilder">StringBuilder chứa ORDER BY clause</param>
        /// <param name="columnSorts">List<ColumnSort> từ request</param>
        private void ApplyColumnSorting(StringBuilder orderBuilder, List<ColumnSortDto> columnSorts)
        {
            var allowedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "work_shift_code",
                "work_shift_name",
                "working_time",
                "break_time",
                "is_active",
                "created_by",
                "modified_by",
                "created_date",
                "modified_date"
            };


            var validSorts = new List<string>();

            foreach (var sort in columnSorts ?? new List<ColumnSortDto>())
            {
                if (string.IsNullOrWhiteSpace(sort.ColumnName))
                    continue;

                var columnName = sort.ColumnName.ToLower();
                if (!allowedColumns.Contains(columnName))
                    continue;

                var direction = (sort.SortDirection ?? "asc").ToLower() == "asc" ? "ASC" : "DESC";
                validSorts.Add($"ws.{columnName} {direction}");
            }

            // Default sort nếu không có sort hợp lệ
            if (!validSorts.Any())
            {
                validSorts.Add("ws.created_date DESC");
            }

            orderBuilder.Append(" ORDER BY " + string.Join(", ", validSorts));
        }


        #endregion
    }
}

