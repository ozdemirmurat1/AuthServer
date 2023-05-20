﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    //[Authorize(Roles ="admin,manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [Authorize(Roles = "admin",Policy ="AnkaraPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            // veritabanında userıd veya userName alanları üzerinden gerekli dataları çek
            // stockId stockQuantity Category UserId/UserName

            return Ok($"Stock işlemleri=> UserName:{userName}-UserId:{userIdClaim.Value}");
        }
    }
}
