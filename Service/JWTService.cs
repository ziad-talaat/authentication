﻿using Authorization_Refreshtoken.DTO;
using Authorization_Refreshtoken.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authorization_Refreshtoken.Service
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _JWToptions;
        private readonly RefreshTokenOptions _refreshTokenOption;
        public JWTService(IConfiguration configuration,IOptions<JwtOptions>jwtoption,IOptions<RefreshTokenOptions>refreshToken)
        {
            _configuration= configuration;
            _JWToptions = jwtoption.Value;   
            _refreshTokenOption= refreshToken.Value;
        }
        public  AuthResponse CreateToken(AppUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddHours(Convert.ToDouble(_JWToptions.Expiration_hours));

            Claim[] claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 

            /////
            new Claim(JwtRegisteredClaimNames.Iat,new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.DateTime),
            //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime), 

            /////
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWToptions.Key));
            SigningCredentials signCred=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
               _JWToptions.Issuer,
               _JWToptions.Audience,
               
                claims,
                expires: expiration,
                signingCredentials: signCred
                );

            JwtSecurityTokenHandler tokenHandler = new();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthResponse
            {
                Token = token,
                ExpirationDate = expiration,
                Email = user.Email,
                UserName = user.UserName,
                RefreshToken = GenerateRefreshToken(),
                ExpirationDateRefreshToken = DateTime.UtcNow.AddDays(Convert.ToInt32(_refreshTokenOption.Expiration_date))
            };
        }

        public string GenerateRefreshToken()
        {
            byte[]bytes=new byte[64];
            var randomNumberGenerator=RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

       

        public ClaimsPrincipal GetPrinciplesFromJWTToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience =_JWToptions.Audience,

                ValidateIssuer = true,
                ValidIssuer = _JWToptions.Issuer,

               // ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWToptions.Key))

            };

            JwtSecurityTokenHandler tokenHandler=new JwtSecurityTokenHandler();

            ClaimsPrincipal principles = tokenHandler.ValidateToken(token, tokenValidationParameters,out SecurityToken validateToken);

            if(validateToken is not JwtSecurityToken jwtsecurityToken || !jwtsecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("InvalidToken");
            }
            return principles;
        }
    }
}
