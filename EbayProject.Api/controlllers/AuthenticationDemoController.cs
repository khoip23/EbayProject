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
//using EbayProject.Api.Models;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationDemoController(EbayContext _context) : ControllerBase
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
            User userDB = _context.Users.SingleOrDefault(u => u.Username == model.UserName || u.Email == model.Email);
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
    }

}