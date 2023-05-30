namespace ParkingApp.DTO
{
    public class ParkingSlotDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsFixed { get; set; }
        public DateTime FixationTime { get; set; }
        public DateTime FixationEndTime { get; set; }
        public string UserLicenesePlate { get; set; }
    }
}
