

using coworking.Helpers;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces.Base;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace coworking.Authorization;

public interface IJwtUtils
{
    public Task<string> GenerateRefreshToken(User user);
    public string GenerateToken(User user);
}

public class JwtUtils : IJwtUtils
{
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;

    public JwtUtils(IOptions<AppSettings> appSettings, IUnitOfWork unitOfWork)
    {
        _appSettings = appSettings.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateRefreshToken(User user)
    {
        var newRefreshToken = new RefreshToken
        {
            Active = true,
            Expiration = DateTime.Now.AddDays(4), ////more longer than access token, user must authenticated again after 5 days if refresh token expired
            RefreshTokenValue = Guid.NewGuid().ToString("N"), //32 digits
            Used = false,
            UserId = user.Id,
            CreatedBy = "A",
            UpdatedBy = "A"
        };

        //saved
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();
        return newRefreshToken.RefreshTokenValue;
    }

    public string GenerateToken(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RolId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(4), //Token will expire, AddHours(4)
            signingCredentials: creds
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;

    }
}