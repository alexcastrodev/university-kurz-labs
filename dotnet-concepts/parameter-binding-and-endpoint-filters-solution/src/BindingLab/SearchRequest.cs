using Microsoft.AspNetCore.Mvc;

namespace BindingLab;

public record SearchRequest(
    [FromRoute] string Category,
    [FromQuery] int Page,
    [FromHeader(Name = "X-Tenant")] string Tenant);
