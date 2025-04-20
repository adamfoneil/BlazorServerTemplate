using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

/// <summary>
/// sample entity to demonstrate Grid UI
/// </summary>
public class Item : BaseTable
{
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public decimal Price { get; set; }
	public DateTime? EffectiveDate { get; set; }
	public bool IsActive { get; set; } = true;
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
	public void Configure(EntityTypeBuilder<Item> builder)
	{
		builder.Property(i => i.Name).HasMaxLength(50);
		builder.Property(i => i.Description).HasMaxLength(200);
		builder.Property(i => i.Price).HasColumnType("money");
		builder.Property(i => i.EffectiveDate).HasColumnType("timestamp without time zone");
	}
}