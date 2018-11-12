using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Services.Authentication.External
{
    public static class JwtHelper
    {
        /// <summary>
        /// 生成JwtToken, 默认超时7天
        /// </summary>
        public static string CreateToken(Claim[] claims, int expires_minutes = 7 * 24 * 60)
        {
            var jwtConfig = EngineContext.Current.Resolve<JwtConfig>();
            string secret = jwtConfig.JwtSecret;
            if (secret == null)
            {
                throw new Exception("创建JwtToken时Secret为空");
            }
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            DateTime now = DateTime.Now;
            DateTime expires = now.AddMinutes(expires_minutes);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = jwtConfig.Audience,
                Issuer = jwtConfig.Issuer,
                SigningCredentials = credentials,
                NotBefore = now,
                IssuedAt = now,
                Expires = expires
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var jwtConfig = EngineContext.Current.Resolve<JwtConfig>();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.JwtSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var param = new TokenValidationParameters()
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,

                // Validate the token expiry
                ValidateLifetime = false,
            };
            var result = tokenHandler.ValidateToken(token, param, out SecurityToken validatedToken);
            return result;
        }
    }
}
