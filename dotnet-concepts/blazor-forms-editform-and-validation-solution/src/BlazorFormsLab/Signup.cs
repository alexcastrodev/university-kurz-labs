using System.ComponentModel.DataAnnotations;

namespace BlazorFormsLab;

public class Signup : IValidatableObject
{
    [Required]
    [StringLength(10, MinimumLength = 3)]
    public string? Name { get; set; }

    [Range(18, 120)]
    public int Age { get; set; } = 18;

    [EmailAddress]
    public string? Email { get; set; }

    public string Plan { get; set; } = "free";

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]
    public bool Terms { get; set; }

    // Model-level rule: runs on submit and on EditContext.Validate(), not when a single field changes.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(Name, "admin", StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("admin is reserved.", [nameof(Name)]);
    }
}
