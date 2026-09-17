namespace metalimes.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation property for players
        public ICollection<Player>? Players { get; set; }
    }
}
