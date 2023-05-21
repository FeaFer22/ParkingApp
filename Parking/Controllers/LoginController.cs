using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ParkingApp.DatabaseContext;
using ParkingApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ParkingApp.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly JWTSettings _options;
        private readonly AppDatabaseContext _databaseContext;

        public LoginController(IOptions<JWTSettings> options, AppDatabaseContext databaseContext)
        {
            _options = options.Value;
            _databaseContext = databaseContext;
        }

        [HttpGet("GetJwtToken")]
        public string GetJwtToken(string licensePlate)
        {
            SqlParameter sqlParameter = new("@licensePlate", licensePlate.ToLower());
            var getUserData = _databaseContext.Users.FromSqlRaw("GetUserByLicensePlate @licensePlate", sqlParameter).ToList();

            User? user = new()
            {
                FullName = getUserData.First().FullName,
                LicensePlate = licensePlate,
                ContactNumber = getUserData.First().ContactNumber
            };

            var claims = new Claim[] {
                    new Claim(ClaimTypes.Name,user.LicensePlate),
                    new Claim(ClaimTypes.Name,user.FullName),
                    new Claim(ClaimTypes.Role, "User")
                };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
