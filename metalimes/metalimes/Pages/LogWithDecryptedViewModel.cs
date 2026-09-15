using metalimes.Data;

namespace metalimes.Pages
{
    /// <summary>
    /// ViewModel for displaying Log entries with decrypted Code field
    /// </summary>
    public class LogWithDecryptedViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? DecryptedCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
