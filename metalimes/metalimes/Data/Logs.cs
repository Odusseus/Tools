namespace metalimes.Data
{
    public class Logs
    {
        public int Id { get; set; }                 // primaire sleutel
        public DateTime Timestamp { get; set; }     // tijdstip van log
        public required string Message { get; set; }         // fix CS8618
        public required string Level { get; set; }           // give a sensible default
        public int? UserId { get; set; }            // optionele koppeling

        public Users? User { get; set; }              // navigatie naar User (nullable because UserId is nullable)

        public Logs(string message, string level = "Info")
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Level = level;
            Timestamp = DateTime.UtcNow;
        }
    }
}
