using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;
using WebApp1.EF;


namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existingUser = await _repo.GetUserByEmail(user.Email);
            if (existingUser != null)
                return BadRequest(new { isExisted = true, error = "User already exists" });

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            user.UserID = await _repo.RegisterUser(user);

            var token = _repo.GenerateJwtToken(user);
            return Ok(new { token, user = new { user.UserID, user.UserName, user.Email, user.Role } });
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User loginInfo)
        {
            if (loginInfo == null || string.IsNullOrEmpty(loginInfo.Email))
                return BadRequest(new { error = "بيانات غير مكتملة" });

            var user = await _repo.GetUserByEmail(loginInfo.Email);
            if (user == null)
                return Unauthorized(new { message = "البريد الإلكتروني غير موجود" });

            if (!BCrypt.Net.BCrypt.EnhancedVerify(loginInfo.Password, user.Password))
                return Unauthorized(new { message = "كلمة المرور غير صحيحة" });

            var token = _repo.GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new { user.UserID, user.UserName, user.Email, user.Role }
            });
        }
    } 
}
