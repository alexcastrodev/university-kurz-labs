using System.ComponentModel.DataAnnotations;

namespace BlazorFormsLab;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NoBannedWordsAttribute : ValidationAttribute
{
    private static readonly string[] Banned = ["root", "system"];

    // TODO-02: reject a string equal (ignore case) to one of Banned with the message
    // "{value} is banned." attached to validationContext.MemberName.
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return ValidationResult.Success;
    }
}
