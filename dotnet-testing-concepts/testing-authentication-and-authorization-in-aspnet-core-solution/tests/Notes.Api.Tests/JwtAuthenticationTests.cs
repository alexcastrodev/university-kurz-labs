using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Notes.Api.Notes;

namespace Notes.Api.Tests;

// The production JWT bearer handler, fed real tokens signed with the test key. Slower to write than the
// fake scheme, but it is the only kind of test that sees expiry, audience and claim mapping.
[TestClass]
[TestCategory("Integration")]
public sealed class JwtAuthenticationTests(TestContext testContext) : IAsyncDisposable
{
    private readonly NotesApiFactory _api = new(useFakeAuthentication: false);

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    private static string Token(string userId, string? role = null, string audience = NotesApiFactory.Audience,
        DateTime? expires = null)
    {
        var claims = new Dictionary<string, object> { ["sub"] = userId };
        if (role is not null)
        {
            claims["role"] = role;
        }

        var now = DateTime.UtcNow;
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = NotesApiFactory.Issuer,
            Audience = audience,
            Claims = claims,
            IssuedAt = now.AddHours(-2),
            NotBefore = now.AddHours(-2),
            Expires = expires ?? now.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(NotesApiFactory.SigningKey)),
                SecurityAlgorithms.HmacSha256),
        });
    }

    private Task<HttpResponseMessage> GetWithTokenAsync(string path, string token)
    {
        var client = _api.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        return client.GetAsync(path, testContext.CancellationToken);
    }

    [TestMethod]
    public async Task AValidTokenShouldBeAccepted()
    {
        var response = await GetWithTokenAsync("/notes", Token("alice"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task TheSubClaimShouldBecomeTheOwnerOfANewNote()
    {
        // "sub" in the token becomes ClaimTypes.NameIdentifier, which is what the endpoints read.
        var client = _api.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("alice"));

        var response = await client.PostAsJsonAsync("/notes", new CreateNoteRequest("buy milk"), testContext.CancellationToken);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var note = await response.Content.ReadFromJsonAsync<Note>(testContext.CancellationToken);
        Assert.AreEqual("alice", note!.OwnerId);
    }

    [TestMethod]
    public async Task ARoleClaimInTheTokenShouldSatisfyTheAdminPolicy()
    {
        // "role" in the token becomes ClaimTypes.Role, which is what RequireRole checks.
        var response = await GetWithTokenAsync("/admin/stats", Token("root", role: "admin"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task AnExpiredTokenShouldBeRejectedWith401()
    {
        // Expired more than the default five minutes of clock skew ago.
        var response = await GetWithTokenAsync("/notes", Token("alice", expires: DateTime.UtcNow.AddMinutes(-10)));

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        StringAssert.Contains(response.Headers.WwwAuthenticate.ToString(), "The token expired");
    }

    [TestMethod]
    public async Task ATokenForAnotherAudienceShouldBeRejectedWith401()
    {
        var response = await GetWithTokenAsync("/notes", Token("alice", audience: "billing-api"));

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
