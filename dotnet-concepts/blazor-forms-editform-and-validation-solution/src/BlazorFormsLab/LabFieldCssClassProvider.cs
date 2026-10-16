using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFormsLab;

public sealed class LabFieldCssClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        if (!editContext.IsModified(fieldIdentifier)) return "pristine";
        return editContext.GetValidationMessages(fieldIdentifier).Any() ? "touched bad" : "touched ok";
    }
}
