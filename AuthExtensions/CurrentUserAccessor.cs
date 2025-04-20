using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthExtensions;

public class CurrentUserAccessor<TUser>(
	AuthenticationStateProvider authStateProvider)	
	where TUser : IdentityUser, IClaimData, new()
{
	private readonly AuthenticationStateProvider _authState = authStateProvider;	

	private TUser? _currentUser;

	public async Task<TUser?> GetAsync()
	{
		if (_currentUser != null)
		{
			return _currentUser;
		}

		var authState = await _authState.GetAuthenticationStateAsync();
		var user = authState.User;

		if (user.Identity is not null && user.Identity.IsAuthenticated)
		{
			_currentUser = new()
			{
				UserName = user.Identity.Name,
				Email = user.Claims.GetClaimValue(ClaimTypes.Email),
				NormalizedEmail = user.Claims.GetClaimValue(ClaimTypes.Email),
				NormalizedUserName = user.Identity.Name
			};

			_currentUser.FromClaims(user.Claims);
		}

		return _currentUser;
	}
}