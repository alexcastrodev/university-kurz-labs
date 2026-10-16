using Microsoft.AspNetCore.Authorization;

namespace AuthLab;

public record MinimumAgeRequirement(int Age) : IAuthorizationRequirement;

public record OwnerRequirement : IAuthorizationRequirement;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        if (int.TryParse(context.User.FindFirst("age")?.Value, out var age) && age >= requirement.Age)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class OwnerHandler : AuthorizationHandler<OwnerRequirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerRequirement requirement, string owner)
    {
        if (context.User.Identity?.Name == owner)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
