namespace AuthSecurityApi.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string>? Errors { get; set; }
    }
}