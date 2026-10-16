using System.ComponentModel.DataAnnotations;

namespace BlazorFormsLab;

public class UsernameModel
{
    [Required]
    [NoBannedWords]
    public string? UserName { get; set; }
}
