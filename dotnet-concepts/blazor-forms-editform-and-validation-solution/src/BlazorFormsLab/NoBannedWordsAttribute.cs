using System.ComponentModel.DataAnnotations;

namespace BlazorFormsLab;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NoBannedWordsAttribute : ValidationAttribute
{
    private static readonly string[] Banned = ["root", "system"];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string s && Banned.Contains(s, StringComparer.OrdinalIgnoreCase))
            return new ValidationResult($"{s} is banned.",
                validationContext.MemberName is null ? null : [validationContext.MemberName]);
        return ValidationResult.Success;
    }
}
