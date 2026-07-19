This is a set of objects to make it easy to get the current `IdentityUser` in your Blazor apps without additional database queries.

## Overview

Custom user properties are baked into the user's `ClaimsPrincipal` at login time by a claims factory. `CurrentUserAccessor<TUser>` reads those claims from `AuthenticationStateProvider` and hydrates a strongly-typed `TUser` instance — no extra database round-trip needed on each page.

## Setup

### 1. Implement `IClaimData` on your `IdentityUser` type

[`IClaimData`](./IClaimData.cs) requires a single method that maps claims back to your custom properties:

```csharp
public class ApplicationUser : IdentityUser, IClaimData
{
	public int UserId { get; set; }
	public string? TimeZoneId { get; set; }
	public int? CurrentTeamId { get; set; }

	public void SetUserProperties(IEnumerable<Claim> claims)
	{
		UserId = claims.GetClaimValue(nameof(UserId)) is string s && int.TryParse(s, out var id) ? id : 0;
		CurrentTeamId = claims.GetClaimValue(nameof(CurrentTeamId)) is string t && int.TryParse(t, out var tid) ? tid : null;
		TimeZoneId = claims.GetClaimValue(nameof(TimeZoneId));
	}
}
```

`GetClaimValue(string claimType)` is an extension method on `IEnumerable<Claim>` provided by [`Extensions`](./Extensions.cs).

### 2. Create a claims factory

Create a class that extends `UserClaimsPrincipalFactory<TUser>` and overrides `GenerateClaimsAsync` to add your custom claims at login time. These are the same claim names that `SetUserProperties` reads back:

```csharp
public class ApplicationUserClaimsFactory(
	UserManager<ApplicationUser> userManager,
	IOptions<IdentityOptions> optionsAccessor)
	: UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
	protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
	{
		var identity = await base.GenerateClaimsAsync(user);

		identity.AddClaims([
			new(nameof(ApplicationUser.UserId), user.UserId.ToString()),
			new(nameof(ApplicationUser.TimeZoneId), user.TimeZoneId ?? string.Empty),
			new(nameof(ApplicationUser.CurrentTeamId), (user.CurrentTeamId ?? 0).ToString()),
		]);

		return identity;
	}
}
```

### 3. Register services at startup

Call `AddCurrentUserInfo` with both your `IdentityUser` type and your claims factory type:

```csharp
builder.Services.AddCurrentUserInfo<ApplicationUser, ApplicationUserClaimsFactory>();
```

This registers:
- `IUserClaimsPrincipalFactory<TUser>` → your `TClaimsFactory`, so custom claims are added at login
- `CurrentUserAccessor<TUser>` as a scoped service

### 4. Resolve the user once in your layout and cascade it

Inject `CurrentUserAccessor` into your layout (e.g. `MainLayout.razor`), call `GetAsync()` once, and wrap the body in a `<CascadingValue>`:

```razor
@inject CurrentUserAccessor<ApplicationUser> CurrentUser

<CascadingValue Value="user">
	@Body
</CascadingValue>

@code {
	ApplicationUser? user;

	protected override async Task OnInitializedAsync()
	{
		user = await CurrentUser.GetAsync();
	}
}
```

`GetAsync()` returns `TUser?` — `null` when the user is not authenticated.

### 5. Consume the user in child components via `[CascadingParameter]`

Any page or component nested under the layout can receive the user without injecting anything:

```csharp
@code {
	[CascadingParameter] public ApplicationUser? User { get; set; }
}
```

This avoids calling `GetAsync()` repeatedly on every component and keeps user access uniform across the app.

## `GetClaimValue` extension

`Extensions.GetClaimValue(this IEnumerable<Claim> claims, string claimType)` is a convenience helper used both in the claims factory and in `SetUserProperties`:

```csharp
string? value = user.Claims.GetClaimValue(nameof(ApplicationUser.UserId));
```

## Recommendation

Once you have the current user in your component, use it with an EF Core `DbContext` via a `SaveChangesAsync` override. This lets you perform permission checks and auditing as part of normal save operations.
