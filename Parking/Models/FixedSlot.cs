namespace ParkingApp.Models
{
    public class FixedSlot
    {
        public Guid Id { get; set; }
        public DateTime FixationTime { get; set; }

        public ParkingSlot ParkingSlot { get; set; }
        public User User { get; set; }
    }
}
