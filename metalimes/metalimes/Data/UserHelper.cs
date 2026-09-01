namespace metalimes.Data
{
    // One-to-one helper table for User containing sensitive or auxiliary fields
    public class UserHelper
    {
        // Shared primary key with User
        public int Id { get; set; }

        // Plain password field (moved from User)
        public string Password { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}
