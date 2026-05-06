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
using WebApp1.EF;
using WebApp1.Models;


namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;
        private readonly JwtSettings _jwt;
        SqlConnection conn;
        public AuthController(IOptions<JwtSettings> jwt, IWebHostEnvironment env, DataContext context)
        {
            _jwt = jwt.Value;
            _env = env;
            _context = context;
            conn = new SqlConnection(_context.Database.GetConnectionString());
        }
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            bool isExisted = false;
            await conn.OpenAsync();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    DataTable dt = new DataTable();
                    string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
                    string sqlCheck = @"select Email from Users Where Email=@Email";
                    using (SqlCommand cmd = new SqlCommand(sqlCheck, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        if (count > 0)
                        {
                            return BadRequest(new { isExisted = true, error = "User is already existed" });
                        }
                    }
                  
                    string sqlInsert = @"insert into Users (UserName,Email,Password,Role) 
                                    values(@UserName,@Email,@Password,@Role) 
                                    SELECT SCOPE_IDENTITY()";
                    using (SqlCommand cmdin = new SqlCommand(sqlInsert, conn,transaction))
                    {
                        cmdin.Parameters.Clear();
                        cmdin.Parameters.AddWithValue("@UserName", user.UserName);
                        cmdin.Parameters.AddWithValue("@Email", user.Email);
                        cmdin.Parameters.AddWithValue("@Password", hashedPassword);
                        cmdin.Parameters.AddWithValue("@Role", user.Role);
                        user.UserID = Convert.ToInt32(await cmdin.ExecuteScalarAsync());
                    }

                    //create Token 
                    var claims = new[]
                    {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "MyRealEstateApi",
                        audience: "MyRealEstateReactApp",
                        claims: claims,
                        expires: DateTime.Now.AddHours(4),
                        signingCredentials: creds
                    );
                    transaction.Commit();
                    var data = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        user = new
                        {
                            user.UserID,
                            user.UserName,
                            user.Email,
                            user.Role
                        }
                    };
                    
                    return Ok(data);

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Internal Server Error");
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User loginInfo)
        {
            bool islogged = false;
            bool isNull = false;
            DataTable dt = new DataTable();
           
                    if (loginInfo == null || string.IsNullOrEmpty(loginInfo.Email))
                        return BadRequest(new { error = "بيانات الدخول غير مكتملة" });

                    await conn.OpenAsync();

                    string sqlSelect = @"select * from Users where Email=@Email";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", loginInfo.Email);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                    await conn.CloseAsync();

                   if (dt.Rows.Count == 0)
                   return Unauthorized(new { message = "البريد الإلكتروني غير موجود" });

                
                string savedPassword = dt.Rows[0]["Password"].ToString();
                bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(loginInfo.Password, savedPassword);

                if (!isPasswordValid)
                    return Unauthorized(new { message = "كلمة المرور غير صحيحة" });

                if (isPasswordValid)
                {
                    islogged = true;
                }
                var userData = new
                {
                    UserID = Convert.ToInt32(dt.Rows[0]["UserID"]),
                    UserName = dt.Rows[0]["UserName"].ToString(),
                    Email = dt.Rows[0]["Email"].ToString(),
                    Role = dt.Rows[0]["Role"].ToString()
                };
                
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, dt.Rows[0]["UserName"].ToString()),
                    new Claim(ClaimTypes.Role, dt.Rows[0]["Role"].ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "MyRealEstateApi",
                    audience: "MyRealEstateReactApp",
                    claims: claims,
                    expires: DateTime.Now.AddHours(4),
                    signingCredentials: creds
                );
              
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user = userData 
                });
               

        }
    } 
}
