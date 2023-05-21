using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ParkingApp.DatabaseContext;
using ParkingApp.Models;

namespace ParkingApp.Controllers
{
    [ApiController]
    [Route("api/parking")]
    [Authorize]
    public class ParkingController : Controller
    {
        private readonly AppDatabaseContext _databaseContext;

        public ParkingController(AppDatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext;
        }

        //[HttpGet]
        //public List<User> GetUsers()
        //{
        //    var Users = _databaseContext.Users.FromSqlRaw("GetUsers").ToList();
        //    return Users;
        //}

        [HttpGet("GetUserByLicensePlate")]
        public User GetUserByLicensePlate(string licensePlate)
        {
            SqlParameter sqlParameter = new("@licensePlate", licensePlate.ToLower());
            var getUserData = _databaseContext.Users.FromSqlRaw("GetUserByLicensePlate @licensePlate", sqlParameter).ToList();
            
            User? user = new()
            {
                FullName = getUserData.First().FullName,
                ContactNumber = getUserData.First().ContactNumber
            };

            return user;
        }

        [HttpGet("GetAllParkingSlots")]
        public List<ParkingSlot> GetParkingSlots()
        {
            var parkSlots = _databaseContext.ParkingSlots.FromSqlRaw("GetAllParkingSlots").ToList();
            return parkSlots;
        }
    }
}
