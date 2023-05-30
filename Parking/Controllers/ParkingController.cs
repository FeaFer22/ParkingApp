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

        private static FixedSlot _fixedSlotInfo;
        public static FixedSlot FixedSlotInfo
        {
            get { return _fixedSlotInfo; }
            set { _fixedSlotInfo = value; }
        }

        private static User _user = new User();
        public static User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public ParkingController(AppDatabaseContext databaseContext, DapperContext dapperContext)
        {
            this._databaseContext = databaseContext;
            _dapperContext = dapperContext;
            _user = LoginController.User;
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
        public async Task<List<ParkingSlotDTO>> GetAllParkingSlots()
        {
            using var connection = _dapperContext.CreateConnection();
            List<ParkingSlotDTO> parkSlots = (List<ParkingSlotDTO>)await connection.QueryAsync<ParkingSlotDTO>("[dbo].[GetAllParkingSlots]", 
                commandType: CommandType.StoredProcedure);

            return parkSlots;
        }

        [HttpGet("GetFixedSlotWhereUserLicensePlate")]
        public async Task<FixedSlot> GetFixedSlotWhereUserLicensePlate(string licensePlate)
        {
            using var connection = _dapperContext.CreateConnection();
            var fixedSlotInfoList = await connection.QueryAsync<FixedSlot>("[dbo].[GetFixedSlotWhereUserLicensePlate]",
                new { UserLicensePlate = licensePlate },
                commandType: CommandType.StoredProcedure);
            var fixedSlot = fixedSlotInfoList.FirstOrDefault();
            return fixedSlot;
        }

        [HttpGet("CheckUserLicensePlate")]
        public async Task<bool> CheckUserLicensePlate(string licensePlate)
        {
            var slotInfo = await GetFixedSlotWhereUserLicensePlate(licensePlate);

            if (slotInfo == null)
            {
                return false;
            }
            return true;
        }

        [HttpGet("CheckFixationIfExists")]
        public async Task<bool> CheckFixationIfExists(string licensePlate)
        {
            var slotInfo = await GetFixedSlotWhereUserLicensePlate(licensePlate);

            if (slotInfo == null)
            {
                return false;
            }
            return true;
        }


        [HttpPost("FixParkingSlot")]
        public async Task<ActionResult> FixParkingSlot(string selectedSlotName, double fixationHours)
        {
            List<ParkingSlotDTO> parkingSlotsDTO = new List<ParkingSlotDTO>();
            parkingSlotsDTO = await GetAllParkingSlots();
            ParkingSlot selectedSlot = new ParkingSlot();

            foreach (var parkingSlot in parkingSlotsDTO)
            {
                if (parkingSlot.Name == selectedSlotName.ToUpper())
                {
                    if (parkingSlot.IsFixed == true)
                    {
                        return BadRequest("Слот уже занят.");
                    }
                    else
                    {
                        if (User != null)
                        {
                            if (await CheckUserLicensePlate(User.LicensePlate) == false)
                            {
                                selectedSlot.Id = parkingSlot.Id;
                                selectedSlot.Name = parkingSlot.Name.ToUpper();
                                selectedSlot.IsFixed = true;

                                DateTime dateTimeNow = DateTime.Now;
                                FixedSlotInfo = new()
                                {
                                    FixationTime = Convert.ToDateTime(dateTimeNow.ToString("g")),
                                    FixationEndTime = Convert.ToDateTime(dateTimeNow.AddHours(fixationHours).ToString("g")),
                                    UserLicensePlate = LoginController.User.LicensePlate,
                                    ParkingSlotName = selectedSlot.Name.ToUpper()
                                };

                                try
                                {
                                    if (FixedSlotInfo != null)
                                    {
                                        using var connection = _dapperContext.CreateConnection();
                                        connection.ExecuteScalar<FixedSlot>("[dbo].[ListFixParkingSlot]",
                                        new
                                        {
                                            fixationTime = FixedSlotInfo.FixationTime,
                                            fixationEndTime = FixedSlotInfo.FixationEndTime,
                                            parkingSlot = FixedSlotInfo.ParkingSlotName,
                                            userLicensePlate = FixedSlotInfo.UserLicensePlate
                                        },
                                        commandType: CommandType.StoredProcedure);

                                        connection.ExecuteScalar<ParkingSlot>("[dbo].[FixParkingSlot]",
                                        new { selectedSlot.IsFixed, selectedSlot.Name },
                                        commandType: CommandType.StoredProcedure);

                                        return Ok(FixedSlotInfo);
                                    }
                                }
                                catch (Microsoft.Data.SqlClient.SqlException e)
                                {
                                    return BadRequest(e.Message);
                                }
                            }
                            return BadRequest("Невозможно забронировать больше одного места.");
                        }
                        else
                        {
                            return BadRequest("Пользователь не найден.");
                        }
                    }
                }
            }
            return BadRequest("Слот не найден.");
        }

        [HttpPost("UnFixParkingSlot")]
        public async Task<ActionResult> UnFixParkingSlot(string unFixSlotName)
        {
            if(User != null)
            {
                if (await CheckFixationIfExists(User.LicensePlate) == false)
                {
                    return BadRequest("Бронь не найдена или невозможно убрать чужую бронь.");
                }
                if (await CheckUserLicensePlate(User.LicensePlate) == true)
                {
                    using var connection = _dapperContext.CreateConnection();

                    var result = await connection.QueryAsync<ParkingSlot>("[dbo].[UnfixParkingSlot]",
                                                new { parkingSlotName = unFixSlotName },
                                                commandType: CommandType.StoredProcedure);
                    return Ok("Бронь удалена.");
                }
                return BadRequest("Невозможно убрать чужую бронь.");
            }
            return BadRequest("Пользователь не найден");
        }
    }
}
