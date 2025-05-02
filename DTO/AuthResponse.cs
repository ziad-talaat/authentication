namespace Authorization_Refreshtoken.DTO
{
    public class AuthResponse
    {
        public string UserName  { get; set; }
        public string Email  { get; set; }
        public string Token  { get; set; }
        public string RefreshToken  { get; set; }
        public DateTime ExpirationDate  { get; set; }
        public DateTime ExpirationDateRefreshToken  { get; set; }
    }
}
