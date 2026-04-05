using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.Domain;

namespace VotingSystem.Tests;

[TestClass]
public class IntHashMapTests
{
    // ── Add / TryGetValue ─────────────────────────────────────────────────────

    [TestMethod]
    public void TryGetValue_AfterAdd_ReturnsCorrectValue()
    {
        var map = new IntHashMap(5);
        map.Add(1, 100);

        Assert.IsTrue(map.TryGetValue(1, out int val));
        Assert.AreEqual(100, val);
    }

    [TestMethod]
    public void TryGetValue_MultipleEntries_EachReturnsCorrectValue()
    {
        var map = new IntHashMap(5);
        map.Add(1, 10);
        map.Add(2, 20);
        map.Add(3, 30);

        Assert.IsTrue(map.TryGetValue(1, out int v1)); Assert.AreEqual(10, v1);
        Assert.IsTrue(map.TryGetValue(2, out int v2)); Assert.AreEqual(20, v2);
        Assert.IsTrue(map.TryGetValue(3, out int v3)); Assert.AreEqual(30, v3);
    }

    [TestMethod]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        var map = new IntHashMap(4);
        map.Add(7, 99);

        Assert.IsFalse(map.TryGetValue(42, out _));
    }

    [TestMethod]
    public void TryGetValue_EmptyMap_ReturnsFalse()
    {
        var map = new IntHashMap(4);
        Assert.IsFalse(map.TryGetValue(1, out _));
    }

    // ── Collision handling (linear probing) ───────────────────────────────────

    [TestMethod]
    public void Add_CollidingKeys_BothStoredAndRetrievable()
    {
        // IntHashMap(2) → capacity = NextPrime(5) = 5
        // Keys 5 and 10 both hash to slot (5 % 5 = 0) and (10 % 5 = 0) → collision
        var map = new IntHashMap(2);
        map.Add(5,  1);
        map.Add(10, 2);

        Assert.IsTrue(map.TryGetValue(5,  out int a)); Assert.AreEqual(1, a);
        Assert.IsTrue(map.TryGetValue(10, out int b)); Assert.AreEqual(2, b);
    }

    [TestMethod]
    public void Add_ManyCollidingKeys_AllRetrievable()
    {
        // All keys 0,5,10,15,20 hash to slot 0 in a capacity-5 table
        var map = new IntHashMap(3);   // capacity = NextPrime(7) = 7
        map.Add(7,  0);
        map.Add(14, 1);
        map.Add(21, 2);

        Assert.IsTrue(map.TryGetValue(7,  out int r0)); Assert.AreEqual(0, r0);
        Assert.IsTrue(map.TryGetValue(14, out int r1)); Assert.AreEqual(1, r1);
        Assert.IsTrue(map.TryGetValue(21, out int r2)); Assert.AreEqual(2, r2);
    }

    // ── Scale ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public void Add_FiftyItems_AllRetrievable()
    {
        int n = 50;
        var map = new IntHashMap(n);
        for (int i = 1; i <= n; i++)
            map.Add(i, i * 10);

        for (int i = 1; i <= n; i++)
        {
            Assert.IsTrue(map.TryGetValue(i, out int val), $"Key {i} not found");
            Assert.AreEqual(i * 10, val, $"Wrong value for key {i}");
        }
    }

    [TestMethod]
    public void Add_LargeNonSequentialIds_AllRetrievable()
    {
        // Simulate real database IDs that might be non-sequential
        int[] ids = { 101, 205, 307, 412, 599 };
        var map = new IntHashMap(ids.Length);
        for (int i = 0; i < ids.Length; i++)
            map.Add(ids[i], i);

        for (int i = 0; i < ids.Length; i++)
        {
            Assert.IsTrue(map.TryGetValue(ids[i], out int val));
            Assert.AreEqual(i, val);
        }
    }
}
