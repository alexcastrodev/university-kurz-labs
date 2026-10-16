using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFormsLab;

public sealed class LabFieldCssClassProvider : FieldCssClassProvider
{
    // TODO-04: return "pristine" for an unmodified field, "touched bad" for a modified
    // field that has validation messages and "touched ok" for a modified valid field.
    // Then register this provider on the EditContext in SignupForm.razor.
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
        => base.GetFieldCssClass(editContext, fieldIdentifier);
}
