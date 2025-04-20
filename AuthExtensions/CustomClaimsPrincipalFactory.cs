using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AuthExtensions;

/// <summary>
/// add this to your main app startup via interface IUserClaimsPrincipalFactory<TUser>
/// </summary>
public class CustomClaimsPrincipalFactory<TUser>(
	UserManager<TUser> userManager, 
	IOptions<IdentityOptions> optionsAccessor) : UserClaimsPrincipalFactory<TUser>(userManager, optionsAccessor) 
	where TUser : IdentityUser, IClaimData
{
	protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
	{
		var identity = await base.GenerateClaimsAsync(user);
		foreach (var claim in user.ToClaims()) identity.AddClaim(claim);
		return identity;
	}
}
