using System.Net;
using System.Security.Claims;
using System.Text;
using EbayProject.Api.Helpers;
using EbayProject.Api.Middleware;
using EbayProject.Api.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//DI:
//Service của blazor server app
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//Service jwt
//Thêm middleware authentication
var privateKey = builder.Configuration["jwt:Serect-Key"];
var Issuer = builder.Configuration["jwt:Issuer"];
var Audience = builder.Configuration["jwt:Audience"];
// Thêm dịch vụ Authentication vào ứng dụng, sử dụng JWT Bearer làm phương thức xác thực
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Thiết lập các tham số xác thực token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // Kiểm tra và xác nhận Issuer (nguồn phát hành token)
        ValidateIssuer = true,
        ValidIssuer = Issuer, // Biến `Issuer` chứa giá trị của Issuer hợp lệ
                              // Kiểm tra và xác nhận Audience (đối tượng nhận token)
        ValidateAudience = true,
        ValidAudience = Audience, // Biến `Audience` chứa giá trị của Audience hợp lệ
                                  // Kiểm tra và xác nhận khóa bí mật được sử dụng để ký token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
        // Sử dụng khóa bí mật (`privateKey`) để tạo SymmetricSecurityKey nhằm xác thực chữ ký của token
        // Giảm độ trễ (skew time) của token xuống 0, đảm bảo token hết hạn chính xác
        ClockSkew = TimeSpan.Zero,
        // Xác định claim chứa vai trò của user (để phân quyền)
        RoleClaimType = ClaimTypes.Role,
        // Xác định claim chứa tên của user
        NameClaimType = ClaimTypes.Name,
        // Kiểm tra thời gian hết hạn của token, không cho phép sử dụng token hết hạn
        ValidateLifetime = true
    };
});

// Thêm dịch vụ Authorization để hỗ trợ phân quyền người dùng
builder.Services.AddAuthorization();

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

//DI MIDDLE WARE
builder.Services.AddScoped<BlockIpMiddleware>();
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
//SỬ DỤNG MIDDLE WARE TỰ TẠO
app.UseMiddleware<BlockIpMiddleware>();
app.MapControllers();
app.UseHttpsRedirection();

//cấu hình các tệp tĩnh
app.UseStaticFiles();

app.UseAuthentication(); //yêu cầu verify token
app.UseAuthorization(); //yêu cầu verify roles của token

//Sử dụng middleware của blazor map file host để làm file chạy đầu tiên
app.MapBlazorHub(); 
app.MapFallbackToPage("/_Host");

app.Run();