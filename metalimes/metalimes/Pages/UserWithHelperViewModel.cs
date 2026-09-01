using metalimes.Data;

namespace metalimes.Pages
{
    /// <summary>
    /// ViewModel combining User and UserHelper information for display
    /// </summary>
    public class UserWithHelperViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? DecryptedPassword { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
