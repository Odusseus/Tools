using System.ComponentModel.DataAnnotations.Schema;

namespace metalimes.Data
{
    // Represents assignment of a single Role to a User
    public class UserRole
    {
        public int UserId { get; set; }
        public Role Role { get; set; }

        public User? User { get; set; }
    }
}
