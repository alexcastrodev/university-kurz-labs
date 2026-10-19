using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Notes.Api.Tests;

// A fake authentication scheme: the test says who the caller is with two headers.
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string UserHeader = "X-Test-User";
    public const string RolesHeader = "X-Test-Roles";

    // TODO-00: Build the caller from the request headers.
    // - No X-Test-User header: return AuthenticateResult.NoResult(). Not Fail, and above all not a default
    //   user: the sample handler in the docs always signs someone in, and then no test can ever see a 401.
    // - Otherwise: a ClaimTypes.NameIdentifier claim with the user id, one ClaimTypes.Role claim per entry of
    //   the comma-separated X-Test-Roles header, a ClaimsIdentity whose authenticationType is SchemeName
    //   (without it, IsAuthenticated is false), and AuthenticateResult.Success with a ticket for SchemeName.
    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        throw new NotImplementedException("TODO-00");
}
