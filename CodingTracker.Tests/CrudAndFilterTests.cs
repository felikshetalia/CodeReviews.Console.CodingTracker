using CodeReviews.Console.CodingTracker;
using Dapper;
namespace CodingTracker.Tests;

[TestFixture]
[NonParallelizable]
public sealed class CrudAndFilterTests : RepositoryTestBase
{
    private static readonly List<TestCaseData> sessionTestCases = [
        new TestCaseData(
                new DateTime(2026, 7, 14, 9, 0, 0),
                new DateTime(2026, 7, 14, 10, 30, 0)
            ),

            new TestCaseData(
                new DateTime(2026, 7, 14, 23, 30, 0),
                new DateTime(2026, 7, 15, 0, 45, 0)
            ),

            new TestCaseData(
                new DateTime(2026, 1, 31, 22, 0, 0),
                new DateTime(2026, 2, 1, 1, 0, 0)
            ),

            new TestCaseData(
                new DateTime(2025, 12, 31, 23, 0, 0),
                new DateTime(2026, 1, 1, 2, 0, 0)
            ),

            new TestCaseData(
                new DateTime(2026, 7, 14, 12, 0, 15),
                new DateTime(2026, 7, 14, 12, 0, 45)
            ),
    ];

    [Test]
    public void GetAll_NewDatabase_ReturnsEmptyList()
    {
        List<CodingSession> sessions = repository.GetAll();
        Assert.That(sessions, Is.Empty);
    }

    [TestCaseSource(nameof(sessionTestCases))]
    public void Add_ValidSession_SavesSession(DateTime start, DateTime end)
    {
        var session = new CodingSession
        {
            StartTime = start,
            EndTime = end
        };

        repository.Add(session);

        List<CodingSession> result = repository.GetAll();

        Assert.That(result, Has.Count.EqualTo(1));

        CodingSession saved = result.Single();

        Assert.Multiple(() =>
        {
            Assert.That(saved.Id, Is.GreaterThan(0));
            Assert.That(saved.StartTime, Is.EqualTo(session.StartTime));
            Assert.That(saved.EndTime, Is.EqualTo(session.EndTime));
            Assert.That(saved.Duration, Is.EqualTo(session.EndTime - session.StartTime));
        });
    }

    [Test]
    public void Add_NullSession_ThrowsArgumentNullException()
    {
        Assert.That(() => repository.Add(null!), Throws.TypeOf<ArgumentNullException>());
    }

    [TestCaseSource(nameof(sessionTestCases))]
    public void GetOne_ExistingId_ReturnsSession(DateTime start, DateTime end)
    {
        var session = new CodingSession
        {
            StartTime = start,
            EndTime = end
        };

        repository.Add(session);

        long savedId = repository.GetAll().Single().Id;

        CodingSession? result = repository.GetOne(savedId);

        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(savedId));
            Assert.That(result.StartTime, Is.EqualTo(session.StartTime));
            Assert.That(result.EndTime, Is.EqualTo(session.EndTime));
        });
    }

    [Test]
    public void GetOne_NonexistentId_ReturnsNull()
    {
        CodingSession? result = repository.GetOne(999999L);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAll_MultipleSessions_ReturnsNewestFirst()
    {
        var oldest = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 12, 10, 0, 0),
            EndTime = new DateTime(2026, 7, 12, 11, 0, 0)
        };

        var newest = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 10, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 11, 0, 0)
        };

        var middle = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 13, 10, 0, 0),
            EndTime = new DateTime(2026, 7, 13, 11, 0, 0)
        };

        repository.Add(oldest);
        repository.Add(newest);
        repository.Add(middle);

        List<CodingSession> result = repository.GetAll();

        DateTime[] actualOrder = result.Select(x => x.StartTime).ToArray();

        DateTime[] expectedOrder =
        [
            newest.StartTime,
            middle.StartTime,
            oldest.StartTime
        ];

        Assert.That(actualOrder, Is.EqualTo(expectedOrder));
    }

    [Test]
    public void Update_ExistingSession_ChangesStoredTimes()
    {
        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        });

        CodingSession stored = repository.GetAll().Single();

        stored.StartTime = new DateTime(2026, 7, 14, 12, 30, 0);
        stored.EndTime = new DateTime(2026, 7, 14, 14, 15, 0);

        bool updated = repository.Update(stored);

        CodingSession? reloaded = repository.GetOne(stored.Id);

        Assert.That(updated, Is.True);
        Assert.That(reloaded, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(reloaded!.StartTime, Is.EqualTo(stored.StartTime));
            Assert.That(reloaded.EndTime, Is.EqualTo(stored.EndTime));
        });
    }

    [Test]
    public void Update_NonexistentSession_ReturnsFalse()
    {
        var session = new CodingSession
        {
            Id = 99999L,
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        bool result = repository.Update(session);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Delete_ExistingSession_RemovesSession()
    {
        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        });

        long id = repository.GetAll().Single().Id;

        bool deleted = repository.Delete(id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.True);
            Assert.That(repository.GetOne(id), Is.Null);
            Assert.That(repository.GetAll(), Is.Empty);
        });
    }

    [Test]
    public void Delete_NonexistentId_ReturnsFalse()
    {
        bool result = repository.Delete(999_999);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Delete_ExistingSession_RemovesOnlySelectedSession()
    {
        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        });

        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 15, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 15, 9, 0, 0)
        });

        List<CodingSession> stored = repository.GetAll();

        long deletedId = stored[0].Id;
        long remainingId = stored[1].Id;

        bool result = repository.Delete(deletedId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(repository.GetOne(deletedId), Is.Null);
            Assert.That(repository.GetOne(remainingId), Is.Not.Null);
            Assert.That(repository.GetAll(), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Update_ExistingSession_ChangesOnlySelectedSession()
    {
        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        });

        repository.Add(new CodingSession
        {
            StartTime = new DateTime(2026, 7, 15, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 15, 9, 0, 0)
        });

        List<CodingSession> stored = repository.GetAll();

        CodingSession target = stored.Single(session => session.StartTime.Day == 14);

        CodingSession untouched = stored.Single(session => session.StartTime.Day == 15);

        DateTime originalUntouchedStart = untouched.StartTime;
        DateTime originalUntouchedEnd = untouched.EndTime;

        target.StartTime = new DateTime(2026, 7, 14, 12, 0, 0);
        target.EndTime = new DateTime(2026, 7, 14, 14, 0, 0);

        bool result = repository.Update(target);

        CodingSession? updated = repository.GetOne(target.Id);

        CodingSession? unchanged = repository.GetOne(untouched.Id);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(updated!.StartTime, Is.EqualTo(target.StartTime));
            Assert.That(updated.EndTime, Is.EqualTo(target.EndTime));
            Assert.That(unchanged!.StartTime, Is.EqualTo(originalUntouchedStart));
            Assert.That(unchanged.EndTime, Is.EqualTo(originalUntouchedEnd));
        });
    }

    [Test]
    public void GetSessionsInBetweenDates_IncludesSessionAtStartBoundary()
    {
        DateTime start = new(2026, 7, 14, 9, 0, 0);
        DateTime end = new(2026, 7, 14, 12, 0, 0);

        repository.Add(new CodingSession
        {
            StartTime = start,
            EndTime = start.AddHours(1)
        });

        List<CodingSession> result =
            repository.GetSessionsInBetweenDates(start, end);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().StartTime, Is.EqualTo(start));
    }

    [Test]
    public void Update_NullSession_ThrowsArgumentNullException()
    {
        Assert.That(() => repository.Update(null!), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void GetSessionsByDay_ReturnsOnlySessionsStartingOnGivenDay()
    {
        var atStartOfDay = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 1, 0, 0)
        };

        var duringDay = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 15, 30, 0),
            EndTime = new DateTime(2026, 7, 14, 16, 30, 0)
        };

        var nextDay = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 15, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 15, 1, 0, 0)
        };

        repository.Add(atStartOfDay);
        repository.Add(duringDay);
        repository.Add(nextDay);

        List<CodingSession> result = repository.GetSessionsByDay(new DateTime(2026, 7, 14));

        Assert.That(result, Has.Count.EqualTo(2));

        Assert.That(result.Select(x => x.StartTime), Is.EquivalentTo(new[] { atStartOfDay.StartTime, duringDay.StartTime }));
    }
    [Test]
    public void GetSessionsByMonthOfYear_ReturnsOnlySessionsStartingInGivenMonthAndYear()
    {
        var januaryFirst = new CodingSession
        {
            StartTime = new DateTime(2026, 1, 5, 10, 0, 0),
            EndTime = new DateTime(2026, 1, 5, 11, 0, 0)
        };

        var januaryLast = new CodingSession
        {
            StartTime = new DateTime(2026, 1, 31, 22, 0, 0),
            EndTime = new DateTime(2026, 2, 1, 1, 0, 0)
        };

        var february = new CodingSession
        {
            StartTime = new DateTime(2026, 2, 1, 8, 0, 0),
            EndTime = new DateTime(2026, 2, 1, 9, 0, 0)
        };

        repository.Add(januaryFirst);
        repository.Add(januaryLast);
        repository.Add(february);

        List<CodingSession> result = repository.GetSessionsByMonthOfYear(2026, 1);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(x => x.StartTime), Is.EqualTo(new[] { januaryLast.StartTime, januaryFirst.StartTime }));
    }

    [Test]
    public void GetSessionsInBetweenDates_ReturnsOnlySessionsStartingWithinGivenRange()
    {
        var beforeRange = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        var insideRange = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 10, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 11, 0, 0)
        };

        var atEndBoundary = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 12, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 13, 0, 0)
        };

        repository.Add(beforeRange);
        repository.Add(insideRange);
        repository.Add(atEndBoundary);

        List<CodingSession> result = repository.GetSessionsInBetweenDates(
            new DateTime(2026, 7, 14, 9, 0, 0),
            new DateTime(2026, 7, 14, 12, 0, 0));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().StartTime, Is.EqualTo(insideRange.StartTime));
    }

    [Test]
    public void GetSessionsByYear_ReturnsOnlySessionsStartingInGivenYear()
    {
        var previousYear = new CodingSession
        {
            StartTime = new DateTime(2025, 12, 31, 23, 0, 0),
            EndTime = new DateTime(2026, 1, 1, 1, 0, 0)
        };

        var targetYear = new CodingSession
        {
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        var nextYear = new CodingSession
        {
            StartTime = new DateTime(2027, 1, 1, 8, 0, 0),
            EndTime = new DateTime(2027, 1, 1, 9, 0, 0)
        };

        repository.Add(previousYear);
        repository.Add(targetYear);
        repository.Add(nextYear);

        List<CodingSession> result = repository.GetSessionsByYear(2026);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().StartTime, Is.EqualTo(targetYear.StartTime));
    }
}
