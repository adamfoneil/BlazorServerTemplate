using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AuthExtensions;

public static class Extensions
{
	/// <summary>
	/// use this in your main app startup to include necessary services to get strong-typed user info in your pages/components
	/// </summary>
	public static void AddCurrentUserInfo<TUser>(this IServiceCollection services)
		where TUser : IdentityUser, IClaimData, new()
	{
		// causes your custom claims to be added to the ClaimsPrincipal when users login
		services.AddScoped<IUserClaimsPrincipalFactory<TUser>, CustomClaimsPrincipalFactory<TUser>>();

		// provides strongly-typed access to the current user info in your pages/components
		services.AddScoped<CurrentUserAccessor<TUser>>();
	}

	public static string? GetClaimValue(this IEnumerable<Claim> claims, string claimType) =>
		claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}
