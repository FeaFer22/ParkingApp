using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ParkingApp.DatabaseContext;
using ParkingApp.DTO;
using ParkingApp.Models;
using System.Diagnostics;

namespace ParkingApp.Controllers
{
    [ApiController]
    [Route("api/parking")]
    [Authorize]
    public class ParkingController : Controller
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly DapperContext _dapperContext;

        public ParkingController(AppDatabaseContext databaseContext, DapperContext dapperContext)
        {
            this._databaseContext = databaseContext;
            _dapperContext = dapperContext;
        }

        [HttpGet("GetUserByLicensePlate")]
        public async Task<IActionResult> GetUserByLicensePlate(string licensePlate)
        {
            using var connection = _dapperContext.CreateConnection();
            var getUserData = await connection.QueryAsync<UserDTO>("[dbo].[GetUserByLicensePlate]", 
                new {LicensePlate = licensePlate.ToUpper()}, commandType:System.Data.CommandType.StoredProcedure);

            if(getUserData != null && getUserData.Count() != 0)
            {
                return Ok(getUserData.FirstOrDefault());
            }
            return BadRequest($"Пользователь с номером {licensePlate} не найден.");
        }

        [HttpGet("GetAllParkingSlots")]
        public List<ParkingSlot> GetParkingSlots()
        {
            var parkSlots = _databaseContext.ParkingSlots.FromSqlRaw("GetAllParkingSlots").ToList();
            return parkSlots;
        }

        //TODO: Add parking slot fixation


    }
}
