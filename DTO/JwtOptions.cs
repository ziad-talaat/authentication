namespace Authorization_Refreshtoken.DTO
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expiration_hours { get; set; }
        public string Key { get; set; }
    }

    public class RefreshTokenOptions
    {
       
        public int Expiration_date { get; set; }
    }
}

