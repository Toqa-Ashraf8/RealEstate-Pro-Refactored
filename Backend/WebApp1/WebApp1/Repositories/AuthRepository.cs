using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;

namespace WebApp1.Repositories
{
    public class AuthRepository:IAuthRepository
    {
        private readonly DbConnection _db;
        private readonly JwtSettings _jwt;

        public AuthRepository(DbConnection db, IOptions<JwtSettings> jwt)
        {
            _db = db;
            _jwt = jwt.Value;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            string sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<int> RegisterUser(User user)
        {
            string sql = @"INSERT INTO Users (UserName, Email, Password, Role) 
                       VALUES (@UserName, @Email, @Password, @Role);
                       SELECT CAST(SCOPE_IDENTITY() as int);";
            return await _db.QuerySingleAsync<int>(sql, user);
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
