I make a lot of Blazor Server apps, and I got tired of building from these scratch. I am not really into Web Assembly. This sample brings together all the boilerplate I start with currently.

# App project
- Uses [Radzen Blazor](https://blazor.radzen.com/) for UI components
- Has a few custom [widgets](https://github.com/adamfoneil/BlazorServerTemplate/tree/main/BlazorApp/Components/Widgets) for common CRUD use and navigation
- I replace the "NoOp" `IEmailSender` with [CoreNotify](https://github.com/adamfoneil/CoreNotify) so that account notification emails work right away. See [startup](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Program.cs#L30). This is technically a paid service of mine. There is a 30-day free trial. There might be a working API key in [configuration](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/appsettings.Development.json#L3) when you're reading this. I recycle this key occasionally, so it might not work in the template.

# Database project
EF Core `IdentityDbContext` project. 
- I added a `TimeZoneId` property to the [ApplicationUser](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationUser.cs) to demonstrate a simple customization
- This can be managed in the profile [manage page](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Components/Account/Pages/Manage/Index.razor#L34)


# Design resources
- Radzen uses [Material Icons](https://fonts.google.com/icons). Anyway there's an `Icon` property, any Google icon will work.
- [flatuicolors.com](https://flatuicolors.com/)
- [colorkit.co](https://colorkit.co/)
