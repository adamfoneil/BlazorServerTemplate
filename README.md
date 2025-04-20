I make a lot of Blazor Server apps, and I got tired of building from these scratch. This sample brings together all the boilerplate I start with currently.

Blazor Server in .NET9 is really nice IMO because of the improved SignalR reconnection experience. In prior .NET versions, that was a bit of a pain point. Disconnections still happen as this is limitation of sockets in browsers, but .NET9 is more graceful in how it reconnects and automatically refreshes the page when needed.

I prefer Blazor Server over Web Assembly because Server is much more productive. In Blazor WASM you have to implement an API layer and split your project into separate backend and frontend projects. That's extra work. In Blazor Server, it's all one project. Also I've had success deploying to DigitalOcean using GitHub Container Registry. This is cheaper than Azure, though there's a little more to setup. I've used Azure a long time, but I don't like being locked into it.

# App project
- Uses [Radzen Blazor](https://blazor.radzen.com/) for UI components
- Has a few custom [widgets](https://github.com/adamfoneil/BlazorServerTemplate/tree/main/BlazorApp/Components/Widgets) for common CRUD use and navigation
- I replace the "NoOp" `IEmailSender` with [CoreNotify](https://github.com/adamfoneil/CoreNotify) so that account notification emails work right away. See [startup](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Program.cs#L30). This is technically a paid service of mine. There is a 30-day free trial. There might be a working API key in [configuration](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/appsettings.Development.json#L3) when you're reading this. I recycle this key occasionally, so it might not work in the template by the time you're reading this. In that case, register your own CoreNotify account and apply your own API key. Info on that is in the CoreNotify readme.
- I recommend [disabling prerendering](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Components/App.razor#L30). When prerendering is turn on, components get initialized multiple times. This is by design, but not efficient if you have database queries being run during component initialization.

# Database project
This uses ASP.NET Identity via an EF Core `IdentityDbContext`.
- I added a `TimeZoneId` property to the [ApplicationUser](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationUser.cs) to demonstrate a simple customization
- This can be managed in the profile [manage page](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Components/Account/Pages/Manage/Index.razor#L34)
- I [link appsettings.json](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/Database.csproj#L11) from the main app to the Database project so that EF migrations can share the same connection string as the app. I don't want to set the connection string in more than one place.
- The db context has simple [auditing](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationDbContext.cs#L31) of inserts and updates
- I use a [BaseTable](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/Conventions/BaseTable.cs) class to ensure consistent `Id` property use across all entities, among other common properties.
- If you need to target different database connections with your migrations, pass an optional additional argument to your `dotnet ef database update` command. For example: `dotnet ef database update -- Dev` will update a connection named `Dev`. Just make sure the connection is defined in your appsettings or local secrets config. This is implemented in the custom [design time db context factory](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationDbContext.cs#L66).

# Auth extensions
Although you can get the `ClaimsPrincipal` in a Blazor app easily, there's no obvious way to get the `ApplicationUser`. This is a problem if you have custom user properties. Although you could query the user from the database, that is not efficient. To address this, I have the [AuthExtensions](https://github.com/adamfoneil/BlazorServerTemplate/tree/main/AuthExtensions) project. This is offered as a NuGet package `AO.Blazor.CurrentUser`, but is also used inline in the template app. Please see its [readme](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/AuthExtensions/readme.md) for more info.

# Install Postgres
I use [this Docker command](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/postgres.txt) to install Postgres locally.

You'll need to make sure Postgres is running before your app is.

# Design resources
- Radzen uses [Material Icons](https://fonts.google.com/icons). Anywhere there's an `Icon` property, any of these Google icons will work.

<details>
  <summary>Learn More</summary>
  Click on the icon preview and use the icon `name` in your Radzen icon property setting.
  
  ![image](https://github.com/user-attachments/assets/ef2d9ab9-0fcd-4c39-b2dc-bf1ccbd2ad23)
</details>

- [flatuicolors.com](https://flatuicolors.com/)
- [colorkit.co](https://colorkit.co/)
