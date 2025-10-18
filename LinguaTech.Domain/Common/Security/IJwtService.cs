using System.Security.Claims;

namespace LinguaTech.Domain.Common.Security;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    ClaimsPrincipal? ValidateToken(string token);
}

