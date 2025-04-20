using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

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
			_currentUser = new();
			_currentUser.FromClaims(user.Claims);
		}

		return _currentUser;
	}
}