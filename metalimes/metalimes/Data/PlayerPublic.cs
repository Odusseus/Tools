namespace metalimes.Data
{
    public class PlayerPublic
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FideId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public PlayerStatus Status { get; set; } = PlayerStatus.New;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int EventId { get; set; }

        public Event? Event { get; set; }
    }
}
