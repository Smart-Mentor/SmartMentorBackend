using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartMentor.Abstraction.Dto;
using SmartMentor.Abstraction.Services.AuthenticationService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Application.Implementations.AuthenticationService
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public JwtTokenService(UserManager<ApplicationUser> userManager,IOptions<JwtSettings> jwtsetting)
        {
            _userManager = userManager;
            _jwtSettings = jwtsetting;
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
           var token = await CreateJwtToken(user);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser
            user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                // Use Jti for unique ID instead of Sub
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
        
                // Single NameIdentifier with USER ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            };

            // Add roles
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Add other claims
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.Value.DurationInDays),
                signingCredentials: credentials
            );
        }

    }
}
