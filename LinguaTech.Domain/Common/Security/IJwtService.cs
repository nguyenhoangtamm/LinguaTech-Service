using System.Security.Claims;

namespace LinguaTech.Domain.Common.Security;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
}

