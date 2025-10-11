using System.Security.Claims;

namespace LinguaTech.Application.Common.Security;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
}

