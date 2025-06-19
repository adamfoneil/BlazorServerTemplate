This is a place for Postgres features that I wanted to be reusable outside the Application.

# DataExporter
If you want to provide a way for users to export data, such as downloading a copy of their data from the app, this is what [DataExporter](https://github.com/adamfoneil/BlazorServerTemplate/blob/main/PostgresExtensions/DataExporter.cs) is for. I don't have a working example yet in the template, but I'll document an example I'm using in another closed-source app. This approach to exporting has two parts:

- Create a Factory class that gathers dependencies needed by your exporter and passes them as arguments to your Exporter. The Factory will live as a transient dependency in your Blazor app.
- Create the Exporter itself, which defines the queries that are run. The Exporter writes output to a Stream, which means it can write to a variety of outputs: a file or -- more practically in a web app -- a downloadable zip file.

1. Implement the [IDataExporterFactory<T>](https://github.com/adamfoneil/FinView/blob/d9a6798a9729a13f6b00fc3a18d66a35e7cb292b/PostgresExtensions/DataExporter.cs#L7) interface in your project. In my initial use case, I wanted to provide a way for the user to download a copy of their data. This means the Exporter needs to know the currently logged in user. In Blazor, the way to get this is via `AuthenticationStateProvider`.

<details>
  <summary>Example</summary>

```csharp
public class UserDataExporterFactory(
  IDbContextFactory<ApplicationDbContext> dbFactory,
  AuthenticationStateProvider authState) : IDataExporterFactory<UserDataExporter>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
    private readonly AuthenticationStateProvider _authState = authState;

    public async Task<UserDataExporter> CreateAsync()
    {
        var authState = await _authState.GetAuthenticationStateAsync();
        var user = authState.User?.Identity?.IsAuthenticated ?? false ? authState.User : throw new Exception("No current user");
        var userName = user.Identity!.Name;

        using var db = _dbFactory.CreateDbContext();
        var userId = await db.Users
            .Where(u => u.UserName == userName)
            .Select(u => u.UserId)
            .SingleOrDefaultAsync();

        if (userId == 0) throw new Exception($"User not found: {userName}");

        return new(_dbFactory, userId);
    }
}
```
</details>

2. Create your Exporter class, implementing `DataExporter`. Here, you define the actual queries that provide the data you want to export by overriding the [GetQueries](https://github.com/adamfoneil/FinView/blob/d9a6798a9729a13f6b00fc3a18d66a35e7cb292b/PostgresExtensions/DataExporter.cs#L16) abstract method. You also receive any dependencies needed by your export process. At minimum this will likely be an EF Core `DbContext` or context factory. It can be anything that can return `IQueryable`s, in fact. In this example, crucially, the other dependency is the current `userId`. Note that I could've made it so the Exporter itself could determine the current user by injecting `AuthenticationStateProvider` there. I didn't do that because it would couple the export functionality to authentication plumbing, undermining its usability in other situations. In other words, it would mean that the `GetQueries` method would require a `userId` argument or require some other way of getting a list of queries to execute.

<details>
  <summary>Example</summary>

```csharp
public class UserDataExporter(IDbContextFactory<ApplicationDbContext> dbFactory, int userId) : DataExporter
{
  private readonly IDbContextFactory<ApplicationDbContext> dbFactory = dbFactory;
  private readonly int _userId = userId;

  protected override IEnumerable<(string Name, IQueryable Query)> GetQueries()
  {
    using var dbContext = dbFactory.CreateDbContext();
    dbContext.Database.GetDbConnection().Open();

    yield return ("Accounts", dbContext.Accounts.Where(row => row.UserId == _userId));
    yield return ("Balances", dbContext.Balances.Include(b => b.Account).Where(b => b.Account!.UserId == _userId));
    yield return ("Goals", dbContext.Goals.Include(g => g.Account).Where(g => g.Account!.UserId == _userId));
    yield return ("GoalDetails", dbContext.GoalDetails.Include(gd => gd.Goal).ThenInclude(g => g.Account).Where(gd => gd.Goal!.Account!.UserId == _userId));
  }
}
```
</details>

3. Add your `IDataExporterFactory` as a transient dependency in your Blazor app. Something like this in your startup:

```csharp
builder.Services.AddTransient<IDataExporterFactory<UserDataExporter>, UserDataExporterFactory>();
```

4. File downloads are not a native Blazor feature, but there's a NuGet package that handles this well [BlazorDownloadFile](https://www.nuget.org/packages/BlazorDownloadFile). Add this to your Blazor app startup as well.

```csharp
builder.Services.AddBlazorDownloadFile();
```

5. In the Blazor page where you want to offer your data download, inject your Factory and the download service.

```csharp
@inject IDataExporterFactory<UserDataExporter> Exporter
@inject IBlazorDownloadFileService Download;
```

6. Create a button or other method of executing the download. In my example, I have a `RadzenButton` with a `Click` event. This uses the [ExportZipAsync](https://github.com/adamfoneil/FinView/blob/d9a6798a9729a13f6b00fc3a18d66a35e7cb292b/PostgresExtensions/ExporterExtensions.cs#L7) extension method to handle the zip file creation. Note how it uses the `IBlazorDownloadFileService` as a lambda within the `ExportZip` method. The reason is that the download operation requires an open stream of some kind as input. By passing the download operation as a lambda, it has access to the zip file stream while it's still open within the `ExportZipAsync` method.

```razor
<RadzenButton Icon="download" Text="My Data" Click="DownloadData" />
```

```csharp
private async Task DownloadData()
{
  var exporter = await Exporter.CreateAsync();
  await exporter.ExportZipAsync(async zip => await Download.DownloadFile("MyData.zip", zip, "application/zip"));
}
```

![image](https://github.com/user-attachments/assets/fd70814c-7bbd-4524-bee5-4c8a3c9c5b79)

![image](https://github.com/user-attachments/assets/9faed7d3-1a85-4bec-a504-5cc323dda85c)

## Next Steps
While this works for exporting raw data, it doesn't provide any complementary import or upload capability. That's the next thing I'd like to implement.
