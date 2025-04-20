using AuthExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Claims;

namespace Database;

public class ApplicationUser : IdentityUser, IClaimData
{
	public string? TimeZoneId { get; set; }

	public void FromClaims(IEnumerable<Claim> claims)
	{
		TimeZoneId = claims.GetClaimValue("TimeZoneId");
		PhoneNumber = claims.GetClaimValue(ClaimTypes.MobilePhone);
	}

	public IEnumerable<Claim> ToClaims()
	{
		if (!string.IsNullOrEmpty(TimeZoneId))
		{
			yield return new Claim("TimeZoneId", TimeZoneId);
		}

		if (!string.IsNullOrEmpty(PhoneNumber))
		{
			yield return new Claim(ClaimTypes.MobilePhone, PhoneNumber);
		}
	}
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(u => u.TimeZoneId).HasMaxLength(50);
	}
}
