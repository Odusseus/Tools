namespace metalimes.Data
{
    public class Users
    {
        public int Id { get; set; }                 // primaire sleutel
        public string Username { get; set; } = string.Empty;        // unieke gebruikersnaam
        public string Password { get; set; } = string.Empty;    // wachtwoord
        public string PasswordHash { get; set; } = string.Empty;    // wachtwoord (gehashed)
        public DateTime CreatedAt { get; set; }     // aanmaakdatum
    }
}
