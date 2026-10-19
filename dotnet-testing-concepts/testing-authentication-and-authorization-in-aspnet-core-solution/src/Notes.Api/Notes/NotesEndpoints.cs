using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Notes.Api.Notes;

public static class NotesEndpoints
{
    public static IEndpointRouteBuilder MapNotesEndpoints(this IEndpointRouteBuilder app)
    {
        var notes = app.MapGroup("/notes").RequireAuthorization();

        notes.MapGet("/", (ClaimsPrincipal user, NoteStore store) => store.OwnedBy(UserId(user)));

        notes.MapPost("/", (CreateNoteRequest request, ClaimsPrincipal user, NoteStore store) =>
        {
            var note = store.Add(UserId(user), request.Text);
            return Results.Created($"/notes/{note.Id}", note);
        });

        notes.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, NoteStore store, IAuthorizationService authorization) =>
        {
            if (store.Find(id) is not { } note)
            {
                return Results.NotFound();
            }

            var result = await authorization.AuthorizeAsync(user, note, Policies.NoteOwner);
            return result.Succeeded ? Results.Ok(note) : Results.Forbid();
        });

        notes.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, NoteStore store, IAuthorizationService authorization) =>
        {
            if (store.Find(id) is not { } note)
            {
                return Results.NotFound();
            }

            var result = await authorization.AuthorizeAsync(user, note, Policies.NoteOwner);
            if (!result.Succeeded)
            {
                return Results.Forbid();
            }

            store.Remove(id);
            return Results.NoContent();
        });

        return app;
    }

    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/admin").RequireAuthorization(Policies.Admin);

        admin.MapGet("/stats", (NoteStore store) => new { Notes = store.Count() });

        return app;
    }

    // The group requires an authenticated user, so the identifier claim is always there.
    private static string UserId(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
