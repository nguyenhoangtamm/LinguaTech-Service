using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using LinguaTech.Application.Common.Security;

namespace LinguaTech.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _settings;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> options)
        {
            _next = next;
            _settings = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = null;

            // Try get token from Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            // Fallback to access_token query (useful for websockets / SignalR)
            if (string.IsNullOrEmpty(token) && context.Request.Query.TryGetValue("access_token", out var accessTokenValues))
            {
                token = accessTokenValues.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_settings.Key);

                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = _settings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                    // attach user to context on successful jwt validation
                    context.User = principal;
                }
                catch
                {
                    // do not attach user if validation fails; request will be handled by authentication/authorization later
                }
            }

            await _next(context);
        }
    }
}
