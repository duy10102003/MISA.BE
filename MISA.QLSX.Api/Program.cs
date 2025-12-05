using MISA.QLSX.Core.Exceptions;
using MISA.QLSX.Core.Interfaces.Repositories;
using MISA.QLSX.Core.Interfaces.Services;
using MISA.QLSX.Core.Services;
using MISA.QLSX.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Cấu hình Dapper để tự động map snake_case sang PascalCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Đăng ký Service
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
//Đăng ký Repo
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IWorkShiftRepo, WorkShiftRepo>();
//Khai báo cross để FE gọi đến 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ValidateExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
