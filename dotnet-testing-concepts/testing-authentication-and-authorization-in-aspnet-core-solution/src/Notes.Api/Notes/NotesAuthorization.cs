using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Notes.Api.Notes;

public static class Policies
{
    public const string Admin = "Admin";
    public const string NoteOwner = "NoteOwner";
}

public static class Roles
{
    public const string Admin = "admin";
    public const string Moderator = "moderator";
}

public sealed class NoteOwnerRequirement : IAuthorizationRequirement;

// Resource-based: the decision needs the note itself, so it runs after the endpoint loaded it.
public sealed class NoteOwnerHandler : AuthorizationHandler<NoteOwnerRequirement, Note>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, NoteOwnerRequirement requirement, Note note)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == note.OwnerId || context.User.IsInRole(Roles.Moderator))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public static class NotesAuthorizationRegistration
{
    public static IServiceCollection AddNotesAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.Admin, policy => policy.RequireRole(Roles.Admin))
            .AddPolicy(Policies.NoteOwner, policy => policy.AddRequirements(new NoteOwnerRequirement()));
        services.AddSingleton<IAuthorizationHandler, NoteOwnerHandler>();
        return services;
    }
}
