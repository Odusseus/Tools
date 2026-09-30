namespace metalimes.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Participants { get; set; } = 0;
        public int Rounds { get; set; } = 0;

        // Navigation property for players
        public ICollection<Player>? Players { get; set; }
        public ICollection<PlayerPublic>? PublicPlayers { get; set; }
    }
}
