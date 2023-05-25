using ParkingApp.DTO;

namespace ParkingApp.Models
{
    public class FixedSlot
    {
        public int Id { get; set; }
        public DateTime FixationTime { get; set; }
        public DateTime FixationEndTime { get; set; }

        public string ParkingSlotName { get; set; } = null!;
        public string UserLicensePlate { get; set; } = null!;

    }
}
