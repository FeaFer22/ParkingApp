using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ParkingApp.DatabaseContext;
using ParkingApp.DTO;
using ParkingApp.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ParkingApp.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly JWTSettings _options;
        private readonly DapperContext _dapperContext;
        public static User user = new User();

        public LoginController(IOptions<JWTSettings> options, DapperContext dapperContext)
        {
            _options = options.Value;
            _dapperContext = dapperContext;
        }

        #region Register

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserEntryDTO userEntry, string firstName, string middleName, string lastName, string contactNumber)
        {
            CreatePasswordHash(userEntry.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.MiddleName = middleName;
            user.ContactNumber = contactNumber;
            user.LicensePlate = userEntry.LicensePlate.ToUpper();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var result = AddUser(user);

            return Ok();
        }

        private IActionResult AddUser(User user)
        {
            using var connection = _dapperContext.CreateConnection();
            var insertUserData = connection.ExecuteScalar<User>("[dbo].[AddUser]", new
            {
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.ContactNumber,
                user.LicensePlate,
                user.PasswordHash,
                user.PasswordSalt
            }, commandType: CommandType.StoredProcedure);
            return Ok();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        #endregion

        #region Login And Creating JWT
        [HttpPost("Login")]
        public async Task<ActionResult> Login(string licensePlate, string password)
        {

            using var connection = _dapperContext.CreateConnection();
            var getUserData = await connection.QueryAsync<User>("[dbo].[GetUserEntry]",
                new { LicensePlate = licensePlate }, commandType: CommandType.StoredProcedure);

            user = getUserData.FirstOrDefault();

            if (user.LicensePlate != licensePlate.ToUpper())
            {
                return BadRequest("Пользователь не найден.");
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Неверный пароль.");
            }

            return Ok(CreateJwt(user));
        }


        [HttpGet("CreateJwt")]
        public string CreateJwt(User user)
        {
            var claims = new Claim[] {
                    new Claim(ClaimTypes.Name,user.LicensePlate),
                    new Claim(ClaimTypes.Role, "User")
                };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

    }
}
