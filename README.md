I make a lot of Blazor Server apps, and I got tired of building from these scratch. This sample brings together all the boilerplate I start with currently.

# Database project
EF Core `IdentityDbContext` project. 
- I added a `TimeZoneId` property to the [ApplicationUser](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/Database/ApplicationUser.cs) to demonstrate a simple customization
- This can be managed in the profile [manage page](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/BlazorApp/Components/Account/Pages/Manage/Index.razor#L34)


# Design resources
- Radzen uses [Material Icons](https://fonts.google.com/icons)
- [flatuicolors.com](https://flatuicolors.com/)
- [colorkit.co](https://colorkit.co/)
