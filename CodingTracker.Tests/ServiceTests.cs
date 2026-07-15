using System.Linq;
using CodeReviews.Console.CodingTracker;

namespace CodingTracker.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ServiceTests
{
    private CodingSessionService _service;
    private CodingSessionRepoDouble _repository;

    private static readonly List<TestCaseData> WeekTestCases =
    [
        new TestCaseData(
            new DateTime(2026, 7, 13, 0, 0, 0),
            new DateTime(2026, 7, 13, 0, 0, 0),
            new DateTime(2026, 7, 20, 0, 0, 0)),

        new TestCaseData(
            new DateTime(2026, 7, 15, 18, 30, 0),
            new DateTime(2026, 7, 13, 0, 0, 0),
            new DateTime(2026, 7, 20, 0, 0, 0)),

        new TestCaseData(
            new DateTime(2026, 7, 19, 12, 0, 0),
            new DateTime(2026, 7, 13, 0, 0, 0),
            new DateTime(2026, 7, 20, 0, 0, 0)),

        new TestCaseData(
            new DateTime(2025, 12, 31, 22, 0, 0),
            new DateTime(2025, 12, 29, 0, 0, 0),
            new DateTime(2026, 1, 5, 0, 0, 0)),
    ];

    private static readonly List<TestCaseData> InvalidIdTestCases = [
        new TestCaseData(0L),
        new TestCaseData(-1L),
        new TestCaseData(-999L),
        new TestCaseData(long.MinValue),
    ];

    [SetUp]
    public void SetUp()
    {
        _repository = new CodingSessionRepoDouble();
        _service = new CodingSessionService(_repository);
    }

    // ADD

    [Test]
    public void Add_ValidTimes_CallsRepositoryWithCreatedSession()
    {
        DateTime startTime = new DateTime(2026, 7, 14, 8, 0, 0);
        DateTime endTime = new DateTime(2026, 7, 14, 10, 0, 0);

        _service.Add(startTime, endTime);

        Assert.That(_repository.AddedSessions, Has.Count.EqualTo(1));

        CodingSession added = _repository.AddedSessions.Single();

        Assert.Multiple(() =>
        {
            Assert.That(added.StartTime, Is.EqualTo(startTime));
            Assert.That(added.EndTime, Is.EqualTo(endTime));
            Assert.That(added.Duration, Is.EqualTo(endTime - startTime));
        });
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void Add_EndNotAfterStart_ThrowsArgumentException(int endOffsetMinutes)
    {
        DateTime startTime = new DateTime(2026, 7, 14, 8, 0, 0);
        DateTime endTime = startTime.AddMinutes(endOffsetMinutes);

        Assert.That(
            () => _service.Add(startTime, endTime),
            Throws.TypeOf<ArgumentException>());

        Assert.That(_repository.AddedSessions, Is.Empty);
    }

    // DELETE

    [TestCaseSource(nameof(InvalidIdTestCases))]
    public void Delete_InvalidId_ReturnsFalseWithoutCallingRepository(long id)
    {
        bool result = _service.Delete(id);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_repository.DeletedId, Is.Null);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Delete_ValidId_ReturnsRepositoryResult(bool repositoryResult)
    {
        _repository.DeleteResult = repositoryResult;

        bool result = _service.Delete(3);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(repositoryResult));
            Assert.That(_repository.DeletedId, Is.EqualTo(3));
        });
    }

    // GET ALL

    [Test]
    public void GetAll_ReturnsSessionsFromRepository()
    {
        var stored = new List<CodingSession>
        {
            new() { Id = 1, StartTime = new DateTime(2026, 7, 14, 8, 0, 0), EndTime = new DateTime(2026, 7, 14, 9, 0, 0) },
            new() { Id = 2, StartTime = new DateTime(2026, 7, 14, 10, 0, 0), EndTime = new DateTime(2026, 7, 14, 11, 0, 0) }
        };

        _repository.SessionsToReturn = stored;

        List<CodingSession> result = _service.GetAll();

        Assert.That(result, Is.SameAs(stored));
    }

    // GET ONE

    [TestCaseSource(nameof(InvalidIdTestCases))]
    public void GetOne_InvalidId_ReturnsNullWithoutCallingRepository(long id)
    {
        CodingSession? result = _service.GetOne(id);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(_repository.GetOneId, Is.Null);
        });
    }

    [Test]
    public void GetOne_ValidIdWhenSessionExists_ReturnsSessionFromRepository()
    {
        var expected = new CodingSession
        {
            Id = 5,
            StartTime = new DateTime(2026, 7, 14, 9, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 10, 0, 0)
        };

        _repository.SessionsToReturn = new List<CodingSession> { expected };

        CodingSession? result = _service.GetOne(expected.Id);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expected));
            Assert.That(_repository.GetOneId, Is.EqualTo(expected.Id));
        });
    }

    [Test]
    public void GetOne_ValidIdWhenSessionDoesNotExist_ReturnsNull()
    {
        _repository.SessionsToReturn = new List<CodingSession>();

        CodingSession? result = _service.GetOne(10);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(_repository.GetOneId, Is.EqualTo(10));
        });
    }

    // UPDATE

    [TestCaseSource(nameof(InvalidIdTestCases))]
    public void Update_InvalidId_ReturnsFalseWithoutCallingRepository(long id)
    {
        bool result = _service.Update(
            id,
            new DateTime(2026, 7, 14, 8, 0, 0),
            new DateTime(2026, 7, 14, 9, 0, 0));

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_repository.GetOneId, Is.Null);
            Assert.That(_repository.UpdatedId, Is.Null);
        });
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void Update_EndNotAfterStart_ThrowsArgumentException(
        int endOffsetMinutes)
    {
        DateTime startTime = new DateTime(2026, 7, 14, 8, 0, 0);
        DateTime endTime = startTime.AddMinutes(endOffsetMinutes);

        Assert.That(
            () => _service.Update(1, startTime, endTime),
            Throws.TypeOf<ArgumentException>());

        Assert.Multiple(() =>
        {
            Assert.That(_repository.GetOneId, Is.Null);
            Assert.That(_repository.UpdatedId, Is.Null);
            Assert.That(_repository.UpdatedSession, Is.Null);
        });
    }

    [Test]
    public void Update_NonexistentSession_ReturnsFalseWithoutCallingUpdate()
    {
        _repository.SessionsToReturn = new List<CodingSession>();

        bool result = _service.Update(
            11,
            new DateTime(2026, 7, 14, 8, 0, 0),
            new DateTime(2026, 7, 14, 9, 0, 0));

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_repository.GetOneId, Is.EqualTo(11));
            Assert.That(_repository.UpdatedId, Is.Null);
        });
    }

    [Test]
    public void Update_ExistingSession_ChangesTimesAndCallsRepositoryUpdate()
    {
        var existing = new CodingSession
        {
            Id = 7,
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        _repository.SessionsToReturn = new List<CodingSession> { existing };

        bool result = _service.Update(
            existing.Id,
            new DateTime(2026, 7, 14, 10, 0, 0),
            new DateTime(2026, 7, 14, 11, 30, 0));

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_repository.UpdatedId, Is.EqualTo(existing.Id));
            Assert.That(_repository.UpdatedSession, Is.Not.Null);
            Assert.That(_repository.UpdatedSession!.StartTime, Is.EqualTo(new DateTime(2026, 7, 14, 10, 0, 0)));
            Assert.That(_repository.UpdatedSession.EndTime, Is.EqualTo(new DateTime(2026, 7, 14, 11, 30, 0)));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Update_ExistingSession_ReturnsRepositoryResult(
        bool repositoryResult)
    {
        var existing = new CodingSession
        {
            Id = 8,
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        _repository.SessionsToReturn = new List<CodingSession> { existing };
        _repository.UpdateResult = repositoryResult;

        bool result = _service.Update(
            existing.Id,
            new DateTime(2026, 7, 14, 10, 0, 0),
            new DateTime(2026, 7, 14, 11, 0, 0));

        Assert.That(result, Is.EqualTo(repositoryResult));
    }

    // FILTER BY DAY

    [Test]
    public void GetByDay_ValidDate_ReturnsSessionsFromRepository()
    {
        var stored = new List<CodingSession>
        {
            new() { Id = 1, StartTime = new DateTime(2026, 7, 14, 8, 0, 0), EndTime = new DateTime(2026, 7, 14, 9, 0, 0) }
        };

        _repository.SessionsToReturn = stored;

        List<CodingSession> result = _service.GetByDay(new DateTime(2026, 7, 14));

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(stored));
            Assert.That(_repository.DayForGetByDay, Is.EqualTo(new DateTime(2026, 7, 14)));
        });
    }

    // FILTER BY WEEK

    [TestCaseSource(nameof(WeekTestCases))]
    public void GetByWeek_ValidDate_UsesCorrectMondayToMondayRange(
        DateTime selectedDate,
        DateTime expectedStart,
        DateTime expectedEnd)
    {
        var stored = new List<CodingSession>
        {
            new() { Id = 1, StartTime = selectedDate, EndTime = selectedDate.AddHours(1) }
        };

        _repository.SessionsToReturn = stored;

        List<CodingSession> result = _service.GetByWeek(selectedDate);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(stored));
            Assert.That(_repository.BetweenDatesArgs?.Start, Is.EqualTo(expectedStart));
            Assert.That(_repository.BetweenDatesArgs?.End, Is.EqualTo(expectedEnd));
        });
    }

    // FILTER BY MONTH AND YEAR

    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(10000, 1)]
    [TestCase(2026, 0)]
    [TestCase(2026, -1)]
    [TestCase(2026, 13)]
    public void GetByMonthOfYear_InvalidArguments_ThrowsArgumentOutOfRangeException(
        int year,
        int month)
    {
        Assert.That(
            () => _service.GetByMonthOfYear(year, month),
            Throws.TypeOf<ArgumentOutOfRangeException>());

        Assert.That(_repository.MonthOfYearArgs, Is.Null);
    }

    [TestCase(2026, 1)]
    [TestCase(2026, 7)]
    [TestCase(2025, 12)]
    public void GetByMonthOfYear_ValidArguments_ReturnsSessionsFromRepository(
        int year,
        int month)
    {
        var stored = new List<CodingSession>
        {
            new() { Id = 1, StartTime = new DateTime(year, month, 1, 8, 0, 0), EndTime = new DateTime(year, month, 1, 9, 0, 0) }
        };

        _repository.SessionsToReturn = stored;

        List<CodingSession> result = _service.GetByMonthOfYear(year, month);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(stored));
            Assert.That(_repository.MonthOfYearArgs, Is.EqualTo((year, month)));
        });
    }

    // FILTER BY YEAR

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(10000)]
    public void GetByYear_InvalidYear_ThrowsArgumentOutOfRangeException(
        int year)
    {
        Assert.That(
            () => _service.GetByYear(year),
            Throws.TypeOf<ArgumentOutOfRangeException>());

        Assert.That(_repository.YearArg, Is.Null);
    }

    [TestCase(1)]
    [TestCase(2026)]
    [TestCase(9999)]
    public void GetByYear_ValidYear_ReturnsSessionsFromRepository(
        int year)
    {
        var stored = new List<CodingSession>
        {
            new() { Id = 1, StartTime = new DateTime(year, 1, 1, 8, 0, 0), EndTime = new DateTime(year, 1, 1, 9, 0, 0) }
        };

        _repository.SessionsToReturn = stored;

        List<CodingSession> result = _service.GetByYear(year);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(stored));
            Assert.That(_repository.YearArg, Is.EqualTo(year));
        });
    }
}
