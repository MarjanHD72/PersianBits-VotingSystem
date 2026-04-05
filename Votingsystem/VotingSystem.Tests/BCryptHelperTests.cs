using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.Domain;

namespace VotingSystem.Tests;

[TestClass]
public class BCryptHelperTests
{
    [TestMethod]
    public void Hash_ThenVerify_CorrectPassword_ReturnsTrue()
    {
        var hash = BCryptHelper.Hash("SecurePass123!");
        Assert.IsTrue(BCryptHelper.Verify("SecurePass123!", hash));
    }

    [TestMethod]
    public void Hash_ThenVerify_WrongPassword_ReturnsFalse()
    {
        var hash = BCryptHelper.Hash("SecurePass123!");
        Assert.IsFalse(BCryptHelper.Verify("wrongpassword", hash));
    }

    [TestMethod]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()
    {
        // Random salt means two hashes of the same password must differ
        var hash1 = BCryptHelper.Hash("password");
        var hash2 = BCryptHelper.Hash("password");
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void Hash_ThenVerify_BothHashes_VerifyCorrectly()
    {
        // Both hashes must still verify against the original password
        var hash1 = BCryptHelper.Hash("password");
        var hash2 = BCryptHelper.Hash("password");
        Assert.IsTrue(BCryptHelper.Verify("password", hash1));
        Assert.IsTrue(BCryptHelper.Verify("password", hash2));
    }

    [TestMethod]
    public void Verify_MalformedStoredHash_ReturnsFalse()
    {
        Assert.IsFalse(BCryptHelper.Verify("password", "notahash"));
    }

    [TestMethod]
    public void Verify_EmptyStoredHash_ReturnsFalse()
    {
        Assert.IsFalse(BCryptHelper.Verify("password", ""));
    }

    [TestMethod]
    public void Hash_OutputContainsColonSeparator()
    {
        // Format must be "salt:hash" (both base64) for Verify to parse correctly
        var hash = BCryptHelper.Hash("test");
        Assert.IsTrue(hash.Contains(':'));
    }
}
