using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EbayProject.Api.Helpers;
using EbayProject.Api.models;
using EbayProject.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

//using EbayProject.Api.Models;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationDemoController(EbayContext _context, JwtAuthService _jwt)
        : ControllerBase
    {
        [Authorize(Roles = "Admin")] //filter
        [HttpGet("getAllUser")]
        public async Task<ActionResult> getAllUser()
        {
            return Ok(await _context.Users.Skip(0).Take(10).ToListAsync());
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterVM model)
        {
            //check email và username có tồn tại hay chưa ?
            User userDB = _context.Users.SingleOrDefault(u =>
                u.Username == model.UserName || u.Email == model.Email
            );
            if (userDB != null)
            {
                return BadRequest("Tài khoản đã tồn tại");
            }
            //Thêm user vào db (Table user)
            User newUser = new User();
            newUser.Username = model.UserName;
            newUser.PasswordHash = PasswordHelper.HashPassword(model.Password);
            newUser.FullName = model.FullName;
            newUser.Email = model.Email;
            newUser.Deleted = false;
            newUser.CreatedAt = DateTime.Now;
            //Thêm user và role vào bảng userRole (để phân quyền)
            UserRole usRole = new UserRole();
            usRole.UserId = newUser.Id;
            usRole.RoleId = RoleId.Buyer;
            newUser.UserRoles.Add(usRole); // Add tham chiếu liên table
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok("Đăng ký thành công!");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] UserLoginVM model)
        {
            //B1: Verify password
            User? us = _context.Users.SingleOrDefault(u =>
                u.Email == model.userNameOrEmail || u.Username == model.userNameOrEmail
            );
            if (us == null)
            {
                return BadRequest("Tài khoản hoặc email không tồn tại!");
            }
            if (!PasswordHelper.VerifyPassword(model.password, us.PasswordHash))
            {
                //Đăng nhập thật bại
                return BadRequest("Sai password!");
            }
            //đúng thì tạo token từ user và role
            string token = _jwt.GenerateToken(us);

            //B2: Tạo token từ thông tin db
            var res = new { token = token };
            //Cấp token qua cookie client
            HttpContext.Response.Cookies.Append(
                "token",
                token,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true, //chỉ gửi cookie qua https
                    SameSite =
                        SameSiteMode.Strict //Ngăn CSRF
                    ,
                }
            );

            return Ok(res);
        }

        [HttpGet("GetCookieClient")]
        public async Task<IActionResult> GetCookieClient()
        {
            string cookie = "";
            bool getCookie = HttpContext.Request.Cookies.TryGetValue(cookie, out cookie);

            return Ok(cookie);
        }

        [HttpPost("DemoFilter")]
        [DemoFilter(name = "abc")]
        public IActionResult DemoFilter([FromBody] UserLoginVM model) //model binding
        {
            //Action excuting

            //Action handler
            Console.WriteLine($@"{JsonSerializer.Serialize(model)}");

            //Action excuted
            UserLoginVM res = model;
            return Ok(res);
        }

        [HttpPost("DemoFilterAsync")]
        [DemoFilterAsync(name = "abc")]
        public async Task<IActionResult> DemoFilterAsync([FromBody] UserLoginVM model) //model binding
        {
            //Action excuting

            //Action handler
            Console.WriteLine($@"{JsonSerializer.Serialize(model)}");

            //Action excuted
            UserLoginVM res = model;
            return Ok(res);
        }

        [HttpPost("DemoSeriLog")]
        public async Task<IActionResult> DemoLog()
        {
            var IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = new LoggerConfiguration()
                .WriteTo.File("Logs/log-action-demo.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information(@$"{IpAddress} - truy cập");
            return Ok("ok");
        }

        [HttpPost("DemoSeriLogFilter")]
        [TypeFilter(typeof(LogAttributeFilter))]
        public IActionResult DemoSeriLogFilter()
        {

            return Ok("ok");
        }
    }
}
