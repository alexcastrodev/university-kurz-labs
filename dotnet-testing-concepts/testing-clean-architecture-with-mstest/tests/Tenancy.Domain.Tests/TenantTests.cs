namespace Tenancy.Domain.Tests;

// TODO-00: The domain needs no setup, no fakes and no framework: construct, act, assert.
// Read src/Tenancy.Domain/Tenant.cs first, then write the tests below.
[TestClass]
public sealed class TenantTests
{
    [TestMethod]
    public void ShouldGenerateATimeOrderedVersion7Id()
    {
        // Construct
        var tenant = new Tenant("test");
        
        // Assert
        // Guid has a Version property.
        Assert.IsExactInstanceOfType<int>(tenant.Id.Version);
    }

    [TestMethod]
    [DataRow("acme", "acme")]
    [DataRow("   ACME-Games", "acme-games")]
    [DataRow("K", "k")]
    public void ShouldNormalizeTheSubdomain(string input, string expected)
    {
        // Construct
        var tenant = new Tenant(input);

        // Several inputs, one expectation each: give the method two string parameters and add [DataRow] rows.
        // Include surrounding spaces and upper case.
        Assert.AreEqual(expected, tenant.Subdomain);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("-acme")] // leading hyphen
    [DataRow("acme-")] // trailing hyphen
    [DataRow("ac.me")]
    [DataRow("ac_me")]
    public void ShouldRejectAnInvalidSubdomain(string input)
    {
        // One test, many bad inputs: empty, blank, leading hyphen, trailing hyphen, a space, a dot, an underscore.
        // Assert.ThrowsExactly<T> wants exactly T. Which type does the constructor throw?
        Assert.ThrowsExactly<ArgumentException>(() => new Tenant(input));
    }

    [TestMethod]
    public void ShouldAcceptExactly63CharactersAndRejectMore()
    {
        // construct
        var tenant = new Tenant(new string('a', 63));
        
        // assert
        // Test both sides of the boundary in the same test.
        Assert.AreEqual(63, tenant.Subdomain.Length);
        Assert.ThrowsExactly<ArgumentException>(() => new Tenant(new string('a', 64)));
    }

    [TestMethod]
    public void ShouldKeepTheIdWhenRenamed()
    {
        // construct
        var tenant = new Tenant("lekito");
        var newName = "lekito-on-rails";
        
        // Act
        tenant.Rename(newName);

        // Assert
        Assert.AreEqual(newName, tenant.Subdomain);
    }

    [TestMethod]
    public void ShouldKeepTheOldSubdomainWhenTheRenameIsInvalid()
    {
        // construct
        var currentName = "lekito";
        var tenant = new Tenant(currentName);
        var newName = "-lekito-on-rails";
        
        // Act
        // A failed operation must not leave the object half changed.
        Assert.ThrowsExactly<ArgumentException>(() => tenant.Rename(newName));

        // Assert
        Assert.AreEqual(currentName, tenant.Subdomain);
    }
}
