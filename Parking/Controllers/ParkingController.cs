using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingApp.DatabaseContext;
using ParkingApp.DTO;
using ParkingApp.Models;
using System.Data;
using System.Runtime.InteropServices;

namespace ParkingApp.Controllers
{
    [ApiController]
    [Route("api/parking")]
    [Authorize]
    public class ParkingController : Controller
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly DapperContext _dapperContext;

        public static UserDTO userDTO;

        private static FixedSlot _fixedSlot;
        public static FixedSlot FixedSlot
        {
            get { return _fixedSlot; }
            set { _fixedSlot = value; }
        }


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
                new {LicensePlate = licensePlate.ToUpper()}, 
                commandType:CommandType.StoredProcedure);

            userDTO = getUserData.FirstOrDefault();

            if (userDTO != null)
            {
                return userDTO;
            }
            return BadRequest($"Пользователь с номером {licensePlate} не найден.");
        }

        [HttpGet("GetAllParkingSlots")]
        public async Task<List<ParkingSlot>> GetParkingSlots()
        {
            using var connection = _dapperContext.CreateConnection();
            var parkSlots = await connection.QueryAsync<ParkingSlot>("[dbo].[GetAllParkingSlots]", 
                commandType: CommandType.StoredProcedure);

            return parkSlots.ToList();
        }

        //TODO: Add parking slot fixation

        [HttpPost("FixParkingSlot")]
        public async Task<ActionResult> FixParkingSlot(string selectedSlotName, double fixationHours)
        {
            List<ParkingSlot> parkingSlots = new List<ParkingSlot>();
            parkingSlots = await GetParkingSlots();
            ParkingSlot selectedSlot = new ParkingSlot();

            foreach (var parkingSlot in parkingSlots)
            {
                if (parkingSlot.Name == selectedSlotName.ToUpper())
                {
                    if(parkingSlot.IsFixed == true)
                    {
                        return BadRequest("Slot already taken.");
                    }
                    else
                    {
                        selectedSlot.Id = parkingSlot.Id;
                        selectedSlot.Name = parkingSlot.Name.ToUpper();
                        selectedSlot.IsFixed = true;

                        DateTime dateTimeNow = DateTime.Now;
                        FixedSlot = new()
                        {
                            FixationTime = Convert.ToDateTime(dateTimeNow.ToString("g")),
                            FixationEndTime = Convert.ToDateTime(dateTimeNow.AddHours(fixationHours).ToString("g")),
                            UserLicensePlate = LoginController.User.LicensePlate,
                            ParkingSlotName = selectedSlot.Name.ToUpper()
                        };

                        if (FixedSlot != null)
                        {
                            using var connection = _dapperContext.CreateConnection();

                            connection.ExecuteScalar<ParkingSlot>("[dbo].[FixParkingSlot]",
                                new { selectedSlot.IsFixed, selectedSlot.Name },
                                commandType: CommandType.StoredProcedure);

                            connection.ExecuteScalar<FixedSlot>("[dbo].[ListFixParkingSlot]",
                                new
                                {
                                    fixationTime = FixedSlot.FixationTime,
                                    fixationEndTime = FixedSlot.FixationEndTime,
                                    parkingSlot = FixedSlot.ParkingSlotName,
                                    userLicensePlate = FixedSlot.UserLicensePlate
                                },
                                commandType: CommandType.StoredProcedure);
                            
                            return Ok(FixedSlot);
                        }
                        else
                        {
                            return BadRequest("FixedSlot is NULL");
                        }
                    }
                }
            }
            return BadRequest("Слот не найден");
        }

        [HttpPost("UnFixParkingSlot")]
        public async Task<ActionResult> UnFixParkingSlot(string parkingSlotName)
        {
            if (LoginController.User.LicensePlate != null)
            {
                using var connection = _dapperContext.CreateConnection();
                connection.ExecuteScalar<FixedSlot>("[dbo].[UnFixParkingSlot]", 
                    new { slotName = parkingSlotName.ToUpper() }, 
                    commandType: CommandType.StoredProcedure);

                connection.ExecuteScalar<ParkingSlot>("[dbo].[FixParkingSlot]",
                    new { IsFixed = false, Name = parkingSlotName.ToUpper() },
                    commandType: CommandType.StoredProcedure);

                return Ok("Success");
            }
            return BadRequest("No such user");
        }
    }
}
