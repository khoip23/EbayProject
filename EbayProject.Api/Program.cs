using System.Net;
using EbayProject.Api.models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//DI
//service controller
builder.Services.AddControllers();

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//ORM EF
string connectionEbay = builder.Configuration.GetConnectionString("ConnectionStringEbay");

builder.Services.AddDbContext<EbayContext>(options =>
    options.UseSqlServer(connectionEbay));

//DI SERVICE CORS
builder.Services.AddCors(option =>
{
    option.AddPolicy("allowLocalHost", builder =>
    {
        builder.WithOrigins("http://localhost:5238")
            .AllowAnyHeader()
            .WithMethods("GET", "POST") //chỉ cho phép get và post cho domain khác
            .AllowCredentials();
    });
});


var app = builder.Build();

//MIDDLEWARE
//Cấu hình middleware bắt lỗi error 
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        // Đặt response content-type thành JSON
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        // Trả về JSON chứa thông tin lỗi
        var errorResponse = new { message = exceptionFeature?.Error.Message ?? "Lỗi không xác định!" };
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});
app.UseCors("allowLocalHost");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseHttpsRedirection();

//cấu hình các tệp tĩnh
app.UseStaticFiles();

app.Run();