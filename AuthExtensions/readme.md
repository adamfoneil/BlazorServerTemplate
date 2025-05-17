Offered as NuGet package `AO.Blazor.CurrentUser`.

This is a set of objects to make it easy to get the current `IdentityUser` in your Blazor apps without additional database queries.

1. Implement [IClaimData](./IClaimData.cs) on your `TUser IdentityUser` type. This defines a conversion between your `IdentityUser` type and a set of claims.

2. In your app startup, call

```csharp
builder.Services.AddCurrentUserInfo<ApplicationUser>();
```

where `ApplicationUser` is your `IdentityUser` type.

3. In your top level `_Imports.razor` component, add this line

```csharp
@inject CurrentUserAccessor<ApplicationUser> CurrentUser
```

Again, substitute your proper `ApplicationUser` type.

Now you can access the current user in any component like this:

```csharp
@code {
	private ApplicationUser user = new();

	protected override async Task OnInitializedAsync()
	{
		user = await CurrentUser.GetAsync();
	}
}
```

Once you have the current user captured in your page/component, I recommend using it with an EF Core `DbContext` via a `SaveChangesAsync` [override](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationDbContext.cs#L25). This way, you can perform permission checks and auditing as part of your normal save operations.
