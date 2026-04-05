namespace VotingSystem.Domain;

/// <summary>
/// A fixed-slot scoring board that accumulates votes in a single O(n) pass.
///
/// Internally backed by parallel arrays (scores[], labels[], imageUrls[]) and a
/// custom open-addressing hash map (IntHashMap) as the slot-index so every Record
/// call is O(1) average. The final ranked output is produced by a single
/// Array.Sort — O(k log k) where k is the number of candidates or options, which
/// is always small.
///
/// Record-call complexity by type:
///   RecordSingleVote    – O(1): one hash-map lookup
///   RecordRating        – O(1): one hash-map lookup
///   RecordMultiChoice   – O(m): one lookup per selected option (m = selections per ballot)
///   RecordBordaRanking  – O(k): one lookup per ranked position
///
/// This replaces the scattered O(n·k) and O(n·k²) LINQ aggregations previously
/// spread across Results.cshtml.cs, centralising all vote-counting logic here.
/// </summary>
public sealed class VoteTally
{
    // ── parallel arrays (one slot per candidate / option) ─────────────────────
    private readonly int[]       _ids;
    private readonly string[]    _labels;
    private readonly string[]    _imageUrls;
    private readonly int[]       _scores;
    private readonly IntHashMap  _slotIndex;   // entity ID → array index, O(1) average lookup
    private int                  _ballotCount;

    // ── construction ─────────────────────────────────────────────────────────

    private VoteTally(int[] ids, string[] labels, string[] imageUrls)
    {
        _ids       = ids;
        _labels    = labels;
        _imageUrls = imageUrls;
        _scores    = new int[ids.Length];
        _slotIndex = new IntHashMap(ids.Length);
        for (int i = 0; i < ids.Length; i++)
            _slotIndex.Add(ids[i], i);
    }

    /// Tally keyed on candidates (carries name + optional photo).
    public static VoteTally ForCandidates(IEnumerable<Candidate> candidates)
    {
        var list = candidates.ToList();
        return new(list.Select(c => c.Id).ToArray(),
                   list.Select(c => c.Name).ToArray(),
                   list.Select(c => c.ImageUrl ?? "").ToArray());
    }

    /// Tally keyed on poll options (text only).
    public static VoteTally ForOptions(IEnumerable<Option> options)
    {
        var list = options.ToList();
        return new(list.Select(o => o.Id).ToArray(),
                   list.Select(o => o.Text).ToArray(),
                   new string[list.Count]);
    }

    /// Tally keyed on rating values 1–5.
    public static VoteTally ForRatingScale() =>
        new([1, 2, 3, 4, 5],
            ["1", "2", "3", "4", "5"],
            new string[5]);

    // ── recording — each method is O(1) per call ──────────────────────────────

    /// Single-choice election vote.
    public void RecordSingleVote(int? candidateId)
    {
        if (candidateId.HasValue && _slotIndex.TryGetValue(candidateId.Value, out int slot))
        {
            _scores[slot]++;
            _ballotCount++;
        }
    }

    /// 1–5 star rating vote.
    public void RecordRating(int? ratingValue)
    {
        if (ratingValue.HasValue && _slotIndex.TryGetValue(ratingValue.Value, out int slot))
        {
            _scores[slot]++;
            _ballotCount++;
        }
    }

    /// Multi-choice vote — parses comma-separated option IDs once, credits each selected slot.
    /// Ballot count is incremented once per voter regardless of how many options they chose.
    public void RecordMultiChoice(string? selectedOptionsCsv)
    {
        if (string.IsNullOrEmpty(selectedOptionsCsv)) return;
        bool counted = false;
        foreach (var part in selectedOptionsCsv.Split(','))
        {
            if (int.TryParse(part, out int id) && _slotIndex.TryGetValue(id, out int slot))
            {
                _scores[slot]++;
                counted = true;
            }
        }
        if (counted) _ballotCount++;
    }

    /// Borda-count ranking — parses the ranked CSV once and awards points in a single pass.
    /// Position 0 (top rank) earns k points, position 1 earns k-1, …, last earns 1.
    /// Previously required an outer loop per option; now runs once per ballot.
    public void RecordBordaRanking(string? rankingOrderCsv)
    {
        if (string.IsNullOrEmpty(rankingOrderCsv)) return;
        var parts = rankingOrderCsv.Split(',');
        for (int pos = 0; pos < parts.Length; pos++)
        {
            if (int.TryParse(parts[pos], out int id) && _slotIndex.TryGetValue(id, out int slot))
                _scores[slot] += _ids.Length - pos;   // top rank earns most points
        }
        _ballotCount++;
    }

    // ── output ────────────────────────────────────────────────────────────────

    /// Number of ballots processed.
    public int BallotCount => _ballotCount;

    /// All entries sorted by score descending.  O(k log k).
    public IReadOnlyList<TallyEntry> GetRankedResults()
    {
        var entries = new TallyEntry[_ids.Length];
        for (int i = 0; i < _ids.Length; i++)
        {
            entries[i] = new TallyEntry(
                Id:         _ids[i],
                Label:      _labels[i],
                ImageUrl:   _imageUrls[i],
                Score:      _scores[i],
                Percentage: _ballotCount > 0
                                ? Math.Round(_scores[i] * 100.0 / _ballotCount, 1)
                                : 0
            );
        }
        Array.Sort(entries, (a, b) => b.Score.CompareTo(a.Score));
        return entries;
    }

    /// Weighted average of slot IDs — used for the rating type average (e.g. 3.7 / 5).
    public double WeightedAverage()
    {
        if (_ballotCount == 0) return 0;
        int sum = 0;
        for (int i = 0; i < _ids.Length; i++) sum += _ids[i] * _scores[i];
        return Math.Round((double)sum / _ballotCount, 1);
    }
}

/// Immutable result entry produced by VoteTally.GetRankedResults().
public readonly record struct TallyEntry(
    int    Id,
    string Label,
    string ImageUrl,
    int    Score,
    double Percentage
);
