using System.Security.Claims;

namespace AuthExtensions;

/// <summary>
/// implement this on your IdentityUser type, including any custom properties
/// that should be available to your application as strongly-type properties
/// </summary>
public interface IClaimData
{
	/// <summary>
	/// this is called when users login to capture custom properties as claims
	/// </summary>	
	IEnumerable<Claim> ToClaims();

	/// <summary>
	/// this is called by CurrentUserAccessor when users visit pages in your app,
	/// and you need strong-typed access to their custom properties
	/// </summary>	
	void FromClaims(IEnumerable<Claim> claims);
}
