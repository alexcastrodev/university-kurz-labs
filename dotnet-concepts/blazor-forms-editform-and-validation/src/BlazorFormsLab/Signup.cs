using System.ComponentModel.DataAnnotations;

namespace BlazorFormsLab;

public class Signup : IValidatableObject
{
    // TODO-00: [Required] and [StringLength(10, MinimumLength = 3)]
    public string? Name { get; set; }

    // TODO-00: [Range(18, 120)]
    public int Age { get; set; } = 18;

    // TODO-00: [EmailAddress]
    public string? Email { get; set; }

    public string Plan { get; set; } = "free";

    // Bonus TODO-06: the terms must be accepted (message: "You must accept the terms.")
    public bool Terms { get; set; }

    // TODO-00: yield a ValidationResult "admin is reserved." for member Name when
    // Name equals "admin" (ignore case).
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}
