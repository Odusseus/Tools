namespace metalimes.Data
{
    public class User
    {
        public int Id { get; set; }                 // primaire sleutel
        public string Username { get; set; } = string.Empty;        // unieke gebruikersnaam
        public string PasswordHash { get; set; } = string.Empty;    // wachtwoord (gehashed)

        // One-to-one helper navigation (contains the plain Password)
        public UserHelper? UserHelper { get; set; }
        // Roles are stored in the UserRole table; use navigation property below
        // public Role Role { get; set; } = Role.Basic;  // previously stored on User

        public List<UserRole> UserRoles { get; set; } = new();

        public DateTime CreatedAt { get; set; }     // aanmaakdatum

        // Status flags
        public bool IsActive { get; set; } = true;   // actief standaard true
        public bool IsBlocked { get; set; } = false; // geblokkeerd standaard false
    }
}
