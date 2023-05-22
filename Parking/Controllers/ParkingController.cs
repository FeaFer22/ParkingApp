using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingApp.DatabaseContext;
using ParkingApp.DTO;
using ParkingApp.Models;
using System.Data;

namespace ParkingApp.Controllers
{
    [ApiController]
    [Route("api/parking")]
    [Authorize]
    public class ParkingController : Controller
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly DapperContext _dapperContext;

        private static UserDTO? userDTO;

        public ParkingController(AppDatabaseContext databaseContext, DapperContext dapperContext)
        {
            this._databaseContext = databaseContext;
            _dapperContext = dapperContext;
        }

        [HttpGet("GetUserByLicensePlate")]
        public async Task<ActionResult<UserDTO>> GetUserByLicensePlate(string licensePlate)
        {
            using var connection = _dapperContext.CreateConnection();
            var getUserData = await connection.QueryAsync<UserDTO>("[dbo].[GetUserByLicensePlate]", 
                new {LicensePlate = licensePlate.ToUpper()}, commandType:CommandType.StoredProcedure);

            userDTO = getUserData.FirstOrDefault();

            if(userDTO != null)
            {
                return userDTO;
            }
            return BadRequest($"Пользователь с номером {licensePlate} не найден.");
        }

        [HttpGet("GetAllParkingSlots")]
        public async Task<List<ParkingSlot>> GetParkingSlots()
        {
            using var connection = _dapperContext.CreateConnection();
            var parkSlots = await connection.QueryAsync<ParkingSlot>("[dbo].[GetAllParkingSlots]", commandType: CommandType.StoredProcedure);
            return parkSlots.ToList();
        }

        //TODO: Add parking slot fixation


    }
}
