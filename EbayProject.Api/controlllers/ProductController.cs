//api-controller
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using System.Data;
using EbayProject.Api.models;
using Microsoft.EntityFrameworkCore;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(EbayContext _context) : ControllerBase
    {
        [HttpGet("GetAllProd")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Products.Skip(0).Take(10).ToListAsync());
        }

        [HttpGet("GetdemobyID/{id}")]
        public async Task Getdemo([FromRoute] string id, [FromHeader] string token)
        {
            string? idRoute = HttpContext.Request.RouteValues["id"] as string;
            string? tokenHeader = HttpContext.Request.Headers["token"];

        }

        [HttpPost("GetdemobyID/{id}")]
        public async Task Getdemo1([FromRoute] string id, [FromHeader] string token, [FromBody] DemoModel models)
        {
            string? idRoute = HttpContext.Request.RouteValues["id"] as string;
            string? tokenHeader = HttpContext.Request.Headers["token"];
            string? ModelID = HttpContext.Request.Form["id"];
            string? ModelName = HttpContext.Request.Form["name"];
        }

    }

}

//tạo 1 class để demo
public class DemoModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}