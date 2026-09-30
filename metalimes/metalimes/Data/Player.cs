namespace metalimes.Data
{
    public enum PlayerStatus
    {
        New = 0,
        Confirmed = 1,
        Cancelled = 2,
        Imported = 3,
        Quit = 4
    }

    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FideId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool IsFideChecked { get; set; } = false;
        public PlayerStatus Status { get; set; } = PlayerStatus.New;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int EventId { get; set; } // Required foreign key

        // Navigation property
        public Event? Event { get; set; }
    }
}
