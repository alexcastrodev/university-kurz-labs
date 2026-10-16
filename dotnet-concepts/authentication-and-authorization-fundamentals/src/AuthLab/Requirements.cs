using Microsoft.AspNetCore.Authorization;

namespace AuthLab;

public record MinimumAgeRequirement(int Age) : IAuthorizationRequirement;

public record OwnerRequirement : IAuthorizationRequirement;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        // TODO-09: Read the "age" claim and call context.Succeed(requirement) when it parses to an
        // age greater than or equal to requirement.Age. Do nothing otherwise: an unmet requirement is a 403.
        return Task.CompletedTask;
    }
}

public class OwnerHandler : AuthorizationHandler<OwnerRequirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerRequirement requirement, string owner)
    {
        // TODO-10: Succeed only when the user's Identity.Name equals the resource (the document owner).
        return Task.CompletedTask;
    }
}
