using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Database.Conventions;

public abstract class BaseTable
{
	public int Id { get; set; }

	[Column(TypeName = "timestamp without time zone")]
	public DateTime DateCreated { get; set; } = DateTime.Now;

	[MaxLength(50)]
	public string CreatedBy { get; set; } = "system";

	[Column(TypeName = "timestamp without time zone")]
	public DateTime? DateModified { get; set; }

	[MaxLength(50)]
	public string? ModifiedBy { get; set; }
}
