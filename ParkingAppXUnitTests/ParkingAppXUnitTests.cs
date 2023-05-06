using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ParkingApp.Controllers;
using ParkingApp.DatabaseContext;
using ParkingApp.Models;
using System.Net.Http.Headers;
using Xunit.Sdk;

namespace ParkingAppXUnitTests
{
    public class ParkingAppXUnitTests
    {
        ParkingController parkingController;
        private readonly AppDatabaseContext _databaseContext;

        [Fact]
        public async Task GetUserByLicensePlate_a001aa001_200returned()
        {
            //arrange
            string licensePlate = "а001аа001";
            parkingController = new ParkingController(_databaseContext);
            
            var expectedValue = new User()
            {
                FullName = "Петров Петр Петрович",
                ContactNumber = "79998886655"
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7118/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("https://localhost:7118/api/parking/GetUserByLicensePlates");
            if (response.IsSuccessStatusCode)
            {
                
            }

            //act
            var actualValue = parkingController.GetUserByLicensePlate(licensePlate);

            //assert
            Assert.Equal(expectedValue, actualValue);
        }
    }
}