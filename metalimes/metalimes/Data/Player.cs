namespace metalimes.Data
{
    public enum PlayerStatus
    {
        New = 0,
        Confirmed = 1
    }

    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public PlayerStatus Status { get; set; } = PlayerStatus.New;
        public int EventId { get; set; } // Required foreign key

        // Navigation property
        public Event? Event { get; set; }
    }
}
