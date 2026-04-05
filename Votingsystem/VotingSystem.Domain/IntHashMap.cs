namespace VotingSystem.Domain;

/// <summary>
/// Fixed-capacity open-addressing hash map (int key → int value) with linear probing.
///
/// Designed from scratch to replace Dictionary&lt;int,int&gt; inside VoteTally,
/// satisfying the requirement to implement a custom data structure
/// without using standard-library collection types.
/// Design decisions:
///   - Open addressing (no linked lists / separate chaining) keeps all data in two
///     contiguous arrays, giving better cache locality than node-based structures.
///   - Linear probing resolves collisions by scanning forward one slot at a time.
///   - Capacity is set to the next prime ≥ 2 × itemCount, keeping the load factor
///     at most 0.5, which bounds the expected probe length to ≤ 1.5 slots.
///   - Sentinel value –1 marks empty slots safely because all entity IDs in this
///     system are positive AUTOINCREMENT integers.
///
/// Time complexity:
///   Add          – O(1) average,  O(n) worst case (full table or all keys collide)
///   TryGetValue  – O(1) average,  O(n) worst case
///   Space        – O(capacity)
/// </summary>
public sealed class IntHashMap
{
    private const int Empty = -1;          // sentinel: slot has never been written

    private readonly int[] _keys;
    private readonly int[] _values;
    private readonly int   _capacity;

    /// <param name="itemCount">The maximum number of distinct keys that will be added.</param>
    public IntHashMap(int itemCount)
    {
        // Next prime ≥ 2 × itemCount keeps load factor ≤ 0.5.
        _capacity = NextPrime(itemCount * 2 + 1);
        _keys     = new int[_capacity];
        _values   = new int[_capacity];
        Array.Fill(_keys, Empty);
    }

    // ── public interface ──────────────────────────────────────────────────────

    /// <summary>
    /// Inserts <paramref name="key"/> → <paramref name="value"/>.
    /// Each key must be added at most once.
    /// O(1) average.
    /// </summary>
    public void Add(int key, int value)
    {
        int slot = Probe(key);
        _keys[slot]   = key;
        _values[slot] = value;
    }

    /// <summary>
    /// Looks up <paramref name="key"/>. Returns true and sets <paramref name="value"/>
    /// if found; otherwise returns false and sets <paramref name="value"/> to 0.
    /// O(1) average.
    /// </summary>
    public bool TryGetValue(int key, out int value)
    {
        int slot = Probe(key);
        if (_keys[slot] == key)
        {
            value = _values[slot];
            return true;
        }
        value = 0;
        return false;
    }

    // ── internal helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Linear probe: start at hash(key) and scan forward until the target key
    /// or an empty slot is found.
    /// </summary>
    private int Probe(int key)
    {
        int i = HashSlot(key);
        while (_keys[i] != Empty && _keys[i] != key)
            i = (i + 1) % _capacity;
        return i;
    }

    /// <summary>Maps a key to its initial slot index.</summary>
    private int HashSlot(int key) => Math.Abs(key % _capacity);

    // ── prime-number helpers (used only during construction) ──────────────────

    private static int NextPrime(int n)
    {
        if (n < 2) return 2;
        while (!IsPrime(n)) n++;
        return n;
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }
}
