using Microsoft.VisualStudio.TestTools.UnitTesting;
using VotingSystem.Domain;

namespace VotingSystem.Tests;

// ── Election (single-choice) ──────────────────────────────────────────────────

[TestClass]
public class VoteTallyElectionTests
{
    private static VoteTally BuildTally() => VoteTally.ForCandidates(new[]
    {
        new Candidate { Id = 1, Name = "Alice", ImageUrl = "/img/alice.jpg" },
        new Candidate { Id = 2, Name = "Bob",   ImageUrl = ""               },
        new Candidate { Id = 3, Name = "Carol", ImageUrl = ""               },
    });

    [TestMethod]
    public void RecordSingleVote_VotesCountedPerCandidate()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(2);

        var results = tally.GetRankedResults().ToDictionary(r => r.Label, r => r.Score);
        Assert.AreEqual(2, results["Alice"]);
        Assert.AreEqual(1, results["Bob"]);
        Assert.AreEqual(0, results["Carol"]);
    }

    [TestMethod]
    public void RecordSingleVote_NullCandidateId_NotCounted()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(null);

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordSingleVote_UnknownCandidateId_NotCounted()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(999);

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void GetRankedResults_SortedDescendingByScore()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(3);          // Carol ×1
        tally.RecordSingleVote(1);          // Alice ×3
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(2);          // Bob ×2
        tally.RecordSingleVote(2);

        var results = tally.GetRankedResults();
        Assert.AreEqual("Alice", results[0].Label);
        Assert.AreEqual("Bob",   results[1].Label);
        Assert.AreEqual("Carol", results[2].Label);
    }

    [TestMethod]
    public void GetRankedResults_PercentagesSumTo100()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(2);
        tally.RecordSingleVote(3);

        double total = tally.GetRankedResults().Sum(r => r.Percentage);
        Assert.AreEqual(100.0, total, delta: 0.2); // rounding tolerance
    }

    [TestMethod]
    public void GetRankedResults_ZeroVotes_AllZeroPercentage()
    {
        var tally = BuildTally();
        Assert.IsTrue(tally.GetRankedResults().All(r => r.Percentage == 0));
    }

    [TestMethod]
    public void GetRankedResults_ImageUrlPreserved()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(1);

        var alice = tally.GetRankedResults().First(r => r.Label == "Alice");
        Assert.AreEqual("/img/alice.jpg", alice.ImageUrl);
    }

    [TestMethod]
    public void BallotCount_MatchesVotesCast()
    {
        var tally = BuildTally();
        tally.RecordSingleVote(1);
        tally.RecordSingleVote(2);
        tally.RecordSingleVote(1);

        Assert.AreEqual(3, tally.BallotCount);
    }
}

// ── Rating scale ──────────────────────────────────────────────────────────────

[TestClass]
public class VoteTallyRatingTests
{
    [TestMethod]
    public void RecordRating_WeightedAverageIsCorrect()
    {
        var tally = VoteTally.ForRatingScale();
        tally.RecordRating(5);
        tally.RecordRating(3);   // average = (5+3)/2 = 4.0

        Assert.AreEqual(4.0, tally.WeightedAverage());
    }

    [TestMethod]
    public void RecordRating_AllFiveStars_AverageIsFive()
    {
        var tally = VoteTally.ForRatingScale();
        for (int i = 0; i < 10; i++) tally.RecordRating(5);

        Assert.AreEqual(5.0, tally.WeightedAverage());
    }

    [TestMethod]
    public void RecordRating_AllOneStar_AverageIsOne()
    {
        var tally = VoteTally.ForRatingScale();
        for (int i = 0; i < 5; i++) tally.RecordRating(1);

        Assert.AreEqual(1.0, tally.WeightedAverage());
    }

    [TestMethod]
    public void RecordRating_NullRating_NotCounted()
    {
        var tally = VoteTally.ForRatingScale();
        tally.RecordRating(null);

        Assert.AreEqual(0, tally.BallotCount);
        Assert.AreEqual(0.0, tally.WeightedAverage());
    }

    [TestMethod]
    public void RecordRating_OutOfRangeValues_Ignored()
    {
        var tally = VoteTally.ForRatingScale();
        tally.RecordRating(0);   // below scale
        tally.RecordRating(6);   // above scale

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void WeightedAverage_ZeroBallots_ReturnsZero()
    {
        Assert.AreEqual(0.0, VoteTally.ForRatingScale().WeightedAverage());
    }

    [TestMethod]
    public void GetRankedResults_FiveStarCountReflectsVotesCast()
    {
        var tally = VoteTally.ForRatingScale();
        tally.RecordRating(5);
        tally.RecordRating(5);
        tally.RecordRating(4);

        var fiveStar = tally.GetRankedResults().First(r => r.Id == 5);
        Assert.AreEqual(2, fiveStar.Score);
    }
}

// ── Multi-choice ──────────────────────────────────────────────────────────────

[TestClass]
public class VoteTallyMultiChoiceTests
{
    private static VoteTally BuildTally() => VoteTally.ForOptions(new[]
    {
        new Option { Id = 10, Text = "Pizza" },
        new Option { Id = 20, Text = "Sushi" },
        new Option { Id = 30, Text = "Tacos" },
    });

    [TestMethod]
    public void RecordMultiChoice_EachOptionCountedCorrectly()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("10,20");   // voter 1: Pizza + Sushi
        tally.RecordMultiChoice("10,30");   // voter 2: Pizza + Tacos
        tally.RecordMultiChoice("20");      // voter 3: Sushi only

        var r = tally.GetRankedResults().ToDictionary(x => x.Label, x => x.Score);
        Assert.AreEqual(2, r["Pizza"]);
        Assert.AreEqual(2, r["Sushi"]);
        Assert.AreEqual(1, r["Tacos"]);
    }

    [TestMethod]
    public void RecordMultiChoice_BallotCountedOncePerVoter()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("10,20,30"); // one voter, three selections

        Assert.AreEqual(1, tally.BallotCount);
    }

    [TestMethod]
    public void RecordMultiChoice_NullCsv_Ignored()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice(null);

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordMultiChoice_EmptyCsv_Ignored()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("");

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordMultiChoice_UnknownOptionId_Ignored()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("999");

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordMultiChoice_SingleOptionCsv_OnlyThatOptionCounted()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("20"); // Sushi only

        var r = tally.GetRankedResults().ToDictionary(x => x.Label, x => x.Score);
        Assert.AreEqual(0, r["Pizza"]);
        Assert.AreEqual(1, r["Sushi"]);
        Assert.AreEqual(0, r["Tacos"]);
    }

    [TestMethod]
    public void RecordMultiChoice_PercentageBasedOnBallotCount()
    {
        var tally = BuildTally();
        tally.RecordMultiChoice("10");  // 1 voter
        tally.RecordMultiChoice("10");  // 2 voters

        var pizza = tally.GetRankedResults().First(r => r.Label == "Pizza");
        Assert.AreEqual(100.0, pizza.Percentage); // both voters chose Pizza
    }
}

// ── Ranking (Borda count) ─────────────────────────────────────────────────────

[TestClass]
public class VoteTallyRankingTests
{
    private static VoteTally BuildTally() => VoteTally.ForOptions(new[]
    {
        new Option { Id = 1, Text = "A" },
        new Option { Id = 2, Text = "B" },
        new Option { Id = 3, Text = "C" },
    });

    [TestMethod]
    public void RecordBordaRanking_SingleVoter_PointsAssignedByPosition()
    {
        // 3 options: pos 0 → 3 pts, pos 1 → 2 pts, pos 2 → 1 pt
        var tally = BuildTally();
        tally.RecordBordaRanking("1,2,3");   // A first, B second, C third

        var r = tally.GetRankedResults().ToDictionary(x => x.Label, x => x.Score);
        Assert.AreEqual(3, r["A"]);
        Assert.AreEqual(2, r["B"]);
        Assert.AreEqual(1, r["C"]);
    }

    [TestMethod]
    public void RecordBordaRanking_TwoVoters_ScoresAccumulate()
    {
        var tally = BuildTally();
        tally.RecordBordaRanking("1,2,3");   // voter 1: A>B>C  →  A:3, B:2, C:1
        tally.RecordBordaRanking("3,1,2");   // voter 2: C>A>B  →  A:2, B:1, C:3

        var r = tally.GetRankedResults().ToDictionary(x => x.Label, x => x.Score);
        Assert.AreEqual(5, r["A"]); // 3+2
        Assert.AreEqual(3, r["B"]); // 2+1
        Assert.AreEqual(4, r["C"]); // 1+3
    }

    [TestMethod]
    public void RecordBordaRanking_ConsensusRanking_WinnerHasHighestScore()
    {
        var tally = BuildTally();
        for (int i = 0; i < 5; i++) tally.RecordBordaRanking("1,2,3"); // everyone agrees A wins

        var results = tally.GetRankedResults();
        Assert.AreEqual("A", results[0].Label);
    }

    [TestMethod]
    public void RecordBordaRanking_NullInput_NotCounted()
    {
        var tally = BuildTally();
        tally.RecordBordaRanking(null);

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordBordaRanking_EmptyString_NotCounted()
    {
        var tally = BuildTally();
        tally.RecordBordaRanking("");

        Assert.AreEqual(0, tally.BallotCount);
    }

    [TestMethod]
    public void RecordBordaRanking_BallotCountIncrementedPerVoter()
    {
        var tally = BuildTally();
        tally.RecordBordaRanking("1,2,3");
        tally.RecordBordaRanking("3,2,1");

        Assert.AreEqual(2, tally.BallotCount);
    }
}
