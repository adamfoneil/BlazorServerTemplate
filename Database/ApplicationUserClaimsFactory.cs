using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Database;

public class ApplicationUserClaimsFactory(
    IDbContextFactory<ApplicationDbContext> dbFactory,
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor) : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var result = await base.GenerateClaimsAsync(user);

        using var db = _dbFactory.CreateDbContext();

        result.AddClaims(await CreateClaimsAsync(db, user));

        return result;
    }

    public static async Task<ApplicationUser> LoadApplicationUserAsync(ApplicationDbContext db, string userName)
    {
        var user = await db.Users.Where(row => row.UserName == userName).SingleOrDefaultAsync() ?? throw new InvalidOperationException("No such user");
        var claims = await CreateClaimsAsync(db, user);
        user.SetUserProperties(claims);
        return user;
    }

    public static async Task<IEnumerable<Claim>> CreateClaimsAsync(ApplicationDbContext db, ApplicationUser user)
    {          
        Claim[] results =
        [            
            new(nameof(ApplicationUser.TimeZoneId), user.TimeZoneId ?? string.Empty),            
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
        ];

        return results;
    }
}