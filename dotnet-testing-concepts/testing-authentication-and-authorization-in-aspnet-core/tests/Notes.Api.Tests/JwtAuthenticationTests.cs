namespace Notes.Api.Tests;

// TODO-08 (optional): The production JWT bearer handler, fed real tokens signed with the test key.
// Use new NotesApiFactory(useFakeAuthentication: false), and mint tokens with
// new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor { Issuer, Audience, Claims, Expires,
// SigningCredentials }) from Microsoft.IdentityModel.JsonWebTokens (it arrives with the JwtBearer package).
// Send them as client.DefaultRequestHeaders.Authorization = new("Bearer", token).
[TestClass]
[TestCategory("Integration")]
public sealed class JwtAuthenticationTests(TestContext testContext)
{
    [TestMethod]
    public async Task AValidTokenShouldBeAccepted()
    {
        _ = testContext;
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    public async Task TheSubClaimShouldBecomeTheOwnerOfANewNote()
    {
        // Create a note with a token whose "sub" is alice. Who owns the note the API returns?
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    public async Task ARoleClaimInTheTokenShouldSatisfyTheAdminPolicy()
    {
        // Put the role in a claim called "role". Does RequireRole("admin") see it?
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    public async Task AnExpiredTokenShouldBeRejectedWith401()
    {
        // The default clock skew is five minutes: expire it longer ago than that. Read the WWW-Authenticate header.
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    public async Task ATokenForAnotherAudienceShouldBeRejectedWith401()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }
}
