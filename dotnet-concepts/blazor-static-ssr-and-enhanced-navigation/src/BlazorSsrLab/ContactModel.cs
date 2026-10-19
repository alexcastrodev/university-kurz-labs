using System.ComponentModel.DataAnnotations;

namespace BlazorSsrLab;

public class ContactModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";
}
