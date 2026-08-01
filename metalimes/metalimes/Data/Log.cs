namespace metalimes.Data
{
    public class Log
    {
        public int Id { get; set; }                 // primaire sleutel
        public DateTime Timestamp { get; set; }     // tijdstip van log
        public required string Message { get; set; }         // fix CS8618
        public required string Level { get; set; }           // give a sensible default
        public int? UserId { get; set; }            // optionele koppeling

        public User? User { get; set; }              // navigatie naar User (nullable because UserId is nullable)

        public Log(string message, string level = "Info")
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Level = level;
            Timestamp = DateTime.UtcNow;
        }
    }
}
