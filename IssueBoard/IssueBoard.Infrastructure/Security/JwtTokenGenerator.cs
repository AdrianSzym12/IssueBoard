using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IssueBoard.Application.Abstractions.Security;
using IssueBoard.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IssueBoard.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResult GenerateToken(User user)
    {
        string issuer = GetRequiredConfigurationValue("Jwt:Issuer");
        string audience = GetRequiredConfigurationValue("Jwt:Audience");
        string signingKey = GetRequiredConfigurationValue("Jwt:SigningKey");
        int expirationMinutes = GetExpirationMinutes();

        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);

        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName)
        };

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(signingKey));
        SigningCredentials credentials = new(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer,
            audience,
            claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResult(accessToken, expiresAtUtc);
    }

    private string GetRequiredConfigurationValue(string key)
    {
        string? value = _configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is missing.");
        }

        return value;
    }

    private int GetExpirationMinutes()
    {
        string value = GetRequiredConfigurationValue("Jwt:ExpirationMinutes");

        if (!int.TryParse(value, out int expirationMinutes) || expirationMinutes <= 0)
        {
            throw new InvalidOperationException("JWT expiration must be a positive number.");
        }

        return expirationMinutes;
    }
}
