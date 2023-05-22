using ParkingApp.DTO;

namespace ParkingApp.Models
{
    public class FixedSlot
    {
        public int Id { get; set; }
        public DateTime FixationTime { get; set; }
        public bool IsFixed { get; set; }

        public ParkingSlot ParkingSlot { get; set; } = null!;
        public User User { get; set; }

    }
}
