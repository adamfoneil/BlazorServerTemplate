using Microsoft.EntityFrameworkCore;
using Service.Database;

namespace BlazorApp.Extensions;

internal static class StartupExtensions
{
	internal static void AddApplicationDb<TDbContext>(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionName = configuration["ConnectionName"] ?? throw new InvalidOperationException("Connection name not found in configuration.");
		var connectionString = configuration.GetConnectionString(connectionName) ?? throw new InvalidOperationException($"Connection string '{connectionName}' not found.");
		services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);
		services.AddDatabaseDeveloperPageExceptionFilter();
	}
}
