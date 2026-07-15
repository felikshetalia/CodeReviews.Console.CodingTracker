using System;
using System.Collections.Generic;
using System.Linq;
using CodeReviews.Console.CodingTracker;

[TestFixture]
public sealed class CodingSessionControllerTests
{
    private FakeCodingSessionView _view;
    private FakeCodingSessionService _service;
    private CodingSessionController _controller;

    [SetUp]
    public void SetUp()
    {
        _view = new FakeCodingSessionView();
        _service = new FakeCodingSessionService();
        _controller = new CodingSessionController(_service, _view);
    }

    [Test]
    public void AddSession_ManualOption_GetsTimesAndCallsServiceAdd()
    {
        _view.AddSessionOptionToReturn = AddSessionOption.Manual;
        _view.SessionTimesToReturn = (
            new DateTime(2026, 7, 14, 8, 0, 0),
            new DateTime(2026, 7, 14, 9, 0, 0));

        _controller.AddSession();

        Assert.Multiple(() =>
        {
            Assert.That(_service.AddCalled, Is.True);
            Assert.That(_service.AddedStartTime, Is.EqualTo(_view.SessionTimesToReturn.StartTime));
            Assert.That(_service.AddedEndTime, Is.EqualTo(_view.SessionTimesToReturn.EndTime));
            Assert.That(_view.LastMessage, Is.EqualTo("Coding session added successfully."));
        });
    }

    [Test]
    public void AddSession_TimerOption_RecordsStartAndEndAndCallsServiceAdd()
    {
        _view.AddSessionOptionToReturn = AddSessionOption.Stopwatch;

        _controller.AddSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.WaitStopwatchCalled, Is.True);
            Assert.That(_service.AddCalled, Is.True);
            Assert.That(_service.AddedStartTime, Is.Not.Null);
            Assert.That(_service.AddedEndTime, Is.Not.Null);
            Assert.That(_service.AddedStartTime, Is.LessThanOrEqualTo(_service.AddedEndTime!.Value));
            Assert.That(_view.LastMessage, Does.StartWith("Session recorded."));
        });
    }

    [Test]
    public void AddSession_BackOption_DoesNotCallService()
    {
        _view.AddSessionOptionToReturn = AddSessionOption.Back;

        _controller.AddSession();

        Assert.That(_service.AddCalled, Is.False);
    }

    [Test]
    public void AddSession_ServiceThrowsArgumentException_DisplaysError()
    {
        _view.AddSessionOptionToReturn = AddSessionOption.Manual;
        _view.SessionTimesToReturn = (
            new DateTime(2026, 7, 14, 8, 0, 0),
            new DateTime(2026, 7, 14, 7, 0, 0));
        _service.AddExceptionToThrow = new ArgumentException("Invalid times");

        _controller.AddSession();

        Assert.Multiple(() =>
        {
            Assert.That(_service.AddCalled, Is.True);
            Assert.That(_view.LastError, Is.EqualTo("Invalid times"));
            Assert.That(_view.DisplayMessageCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void ViewSessions_NoSessions_DisplaysEmptyListAndDoesNotShowFilterMenu()
    {
        _service.SessionsToReturn = new List<CodingSession>();

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(1));
            Assert.That(_view.DisplayedSessions.Single().Count, Is.EqualTo(0));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(0));
        });
    }

    [Test]
    public void ViewSessions_SessionsExist_DisplaysSessionsAndShowsFilterMenu()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(1));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(1));
            Assert.That(_view.DisplayedSessions.Single().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void ViewSessions_ShowAllSelected_CallsGetAllAgain()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.FilterOptionsToReturn.Enqueue(FilterOption.ShowAll);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.GetAllCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
        });
    }

    [Test]
    public void ViewSessions_DaySelected_GetsDateAndCallsGetByDay()
    {
        DateTime requestedDate = new DateTime(2026, 7, 14);
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 99,
                StartTime = new DateTime(2026, 1, 1, 8, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 9, 0, 0)
            }
        ];

        _service.SessionsToReturnForGetByDay = [
            new CodingSession
            {
                Id = 1,
                StartTime = requestedDate,
                EndTime = requestedDate.AddHours(1)
            }
        ];

        _view.FilterDateToReturn = requestedDate;
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Day);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.DayArgument, Is.EqualTo(requestedDate));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
        });
    }

    [Test]
    public void ViewSessions_WeekSelected_GetsDateAndCallsGetByWeek()
    {
        DateTime requestedDate = new DateTime(2026, 7, 16);
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 99,
                StartTime = new DateTime(2026, 7, 10, 8, 0, 0),
                EndTime = new DateTime(2026, 7, 10, 9, 0, 0)
            }
        ];

        _service.SessionsToReturnForGetByWeek = [
            new CodingSession
            {
                Id = 1,
                StartTime = requestedDate,
                EndTime = requestedDate.AddHours(1)
            }
        ];

        _view.FilterDateToReturn = requestedDate;
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Week);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.WeekArgument, Is.EqualTo(requestedDate));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayedSessions[1].Single().Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void ViewSessions_MonthSelected_GetsMonthAndYearAndCallsGetByMonthOfYear()
    {
        Month month = Month.July;
        int year = 2026;
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 99,
                StartTime = new DateTime(2026, 1, 10, 8, 0, 0),
                EndTime = new DateTime(2026, 1, 10, 9, 0, 0)
            }
        ];

        _service.SessionsToReturnForGetByMonth = [
            new CodingSession
            {
                Id = 1,
                StartTime =new DateTime(year, (int)month, 1, 8, 0, 0),
                EndTime = new DateTime(year, (int)month, 1, 9, 0, 0)
            }
        ];

        _view.MonthToReturn = month;
        _view.YearToReturn = year;
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Month);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.MonthArguments, Is.EqualTo((year, (int)month)));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayedSessions[1].Single().Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void ViewSessions_YearSelected_GetsYearAndCallsGetByYear()
    {
        int year = 2026;
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 99,
                StartTime = new DateTime(2025, 7, 10, 8, 0, 0),
                EndTime = new DateTime(2025, 7, 10, 9, 0, 0)
            }
        ];

        _service.SessionsToReturnForGetByYear = [
            new CodingSession
            {
                Id = 1,
                StartTime = new DateTime(year, 1, 1, 8, 0, 0),
                EndTime = new DateTime(year, 1, 1, 9, 0, 0)
            }
        ];

        _view.YearToReturn = year;
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Year);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.YearArgument, Is.EqualTo(year));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayedSessions[1].Single().Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void ViewSessions_BackSelected_ReturnsWithoutFurtherServiceCalls()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_service.GetAllCalls, Is.EqualTo(1));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(1));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public void UpdateSession_NoSessions_DoesNotAskForId()
    {
        _service.SessionsToReturn = new List<CodingSession>();

        _controller.UpdateSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.GetSessionIdCalls, Is.EqualTo(0));
            Assert.That(_view.LastMessage, Is.EqualTo("No coding sessions available to update."));
        });
    }

    [Test]
    public void UpdateSession_SessionsExist_DisplaysSessionsBeforeAskingForId()
    {
        var session = new CodingSession
        {
            Id = 1,
            StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
            EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
        };

        _service.SessionsToReturn = [session];
        _view.SessionIdToReturn = 1;

        _view.SessionTimesToReturn =
        (
            new DateTime(2026, 7, 14, 10, 0, 0),
            new DateTime(2026, 7, 14, 11, 0, 0)
        );

        _controller.UpdateSession();

        Assert.That(_view.Events, Is.EqualTo(new[]{
                                                    ViewEvent.DisplaySessions,
                                                    ViewEvent.GetSessionId,
                                                    ViewEvent.GetSessionTimes
                                                }));
    }

    [Test]
    public void UpdateSession_NonexistentId_DisplaysErrorAndDoesNotAskForTimes()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.SessionIdToReturn = 99;

        _controller.UpdateSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.LastError, Is.EqualTo("Coding session 99 was not found."));
            Assert.That(_view.GetSessionTimesCalls, Is.EqualTo(0));
        });
    }

    [Test]
    public void UpdateSession_ValidId_GetsTimesAndCallsServiceUpdate()
    {
        var stored = new CodingSession { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
        _service.SessionsToReturn = new List<CodingSession> { stored };
        _view.SessionIdToReturn = 1;
        _view.SessionTimesToReturn = (DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));

        _controller.UpdateSession();

        Assert.Multiple(() =>
        {
            Assert.That(_service.UpdatedId, Is.EqualTo(1));
            Assert.That(_service.UpdatedStartTime, Is.EqualTo(_view.SessionTimesToReturn.StartTime));
            Assert.That(_service.UpdatedEndTime, Is.EqualTo(_view.SessionTimesToReturn.EndTime));
            Assert.That(_view.LastMessage, Is.EqualTo("Coding session updated successfully."));
        });
    }

    [Test]
    public void UpdateSession_UpdateReturnsFalse_DisplaysError()
    {
        var stored = new CodingSession { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
        _service.SessionsToReturn = new List<CodingSession> { stored };
        _view.SessionIdToReturn = 1;
        _view.SessionTimesToReturn = (DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));
        _service.UpdateResult = false;

        _controller.UpdateSession();

        Assert.That(_view.LastError, Is.EqualTo("Coding session 1 was not found."));
    }

    [Test]
    public void UpdateSession_InvalidTimes_DisplaysValidationError()
    {
        var stored = new CodingSession { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
        _service.SessionsToReturn = new List<CodingSession> { stored };
        _view.SessionIdToReturn = 1;
        _view.SessionTimesToReturn = (DateTime.Now.AddHours(2), DateTime.Now.AddHours(1));
        _service.UpdateExceptionToThrow = new ArgumentException("Invalid time range");

        _controller.UpdateSession();

        Assert.That(_view.LastError, Is.EqualTo("Invalid time range"));
    }

    [Test]
    public void DeleteSession_NoSessions_DoesNotAskForId()
    {
        _service.SessionsToReturn = new List<CodingSession>();

        _controller.DeleteSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.GetSessionIdCalls, Is.EqualTo(0));
            Assert.That(_view.LastMessage, Is.EqualTo("No coding sessions available to delete."));
        });
    }

    [Test]
    public void DeleteSession_SessionsExist_DisplaysSessionsBeforeAskingForId()
    {
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 2,
                StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
                EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
            }
        ];

        _view.SessionIdToReturn = 2;

        _controller.DeleteSession();

        Assert.That(_view.Events, Is.EqualTo(new[] { ViewEvent.DisplaySessions,
                                                        ViewEvent.GetSessionId
                                                    }));
    }

    [Test]
    public void DeleteSession_ExistingId_CallsServiceDelete()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.SessionIdToReturn = 2;

        _controller.DeleteSession();

        Assert.That(_service.DeletedId, Is.EqualTo(2));
    }

    [Test]
    public void DeleteSession_DeleteReturnsTrue_DisplaysSuccessMessage()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.SessionIdToReturn = 2;
        _service.DeleteResult = true;

        _controller.DeleteSession();

        Assert.That(_view.LastMessage, Is.EqualTo("Coding session deleted successfully."));
    }

    [Test]
    public void DeleteSession_DeleteReturnsFalse_DisplaysError()
    {
        _service.SessionsToReturn = new List<CodingSession>
        {
            new() { Id = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };
        _view.SessionIdToReturn = 2;
        _service.DeleteResult = false;

        _controller.DeleteSession();

        Assert.That(_view.LastError, Is.EqualTo("Coding session 2 was not found."));
    }

    [Test]
    public void ViewSessions_DataRetrievalFails_DisplaysErrorAndReturns()
    {
        _service.GetAllExceptionToThrow = new InvalidOperationException("boom");

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_view.LastError, Is.EqualTo("Unexpected error: boom"));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(0));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(0));
        });
    }

    [Test]
    public void UpdateSession_DataRetrievalFails_DisplaysErrorAndReturns()
    {
        _service.GetAllExceptionToThrow = new InvalidOperationException("boom");

        _controller.UpdateSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.LastError, Is.EqualTo("Unexpected error: boom"));
            Assert.That(_view.GetSessionIdCalls, Is.EqualTo(0));
        });
    }

    [Test]
    public void DeleteSession_DataRetrievalFails_DisplaysErrorAndReturns()
    {
        _service.GetAllExceptionToThrow = new InvalidOperationException("boom");

        _controller.DeleteSession();

        Assert.Multiple(() =>
        {
            Assert.That(_view.LastError, Is.EqualTo("Unexpected error: boom"));
            Assert.That(_view.GetSessionIdCalls, Is.EqualTo(0));
        });
    }

    [Test]
    public void ViewSessions_FilterReturnsEmptyList_StaysInFilteringLoop()
    {
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 1,
                StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
                EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
            }
        ];

        _service.SessionsToReturnForGetByDay = [];

        _view.FilterDateToReturn = new DateTime(2020, 1, 1);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Day);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Back);

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(2));
            Assert.That(_view.DisplayedSessions[1], Is.Empty);
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(2));
        });
    }

    [Test]
    public void ViewSessions_FilterRetrievalFails_DisplaysErrorAndReturns()
    {
        _service.SessionsToReturn = [
            new CodingSession
            {
                Id = 1,
                StartTime = new DateTime(2026, 7, 14, 8, 0, 0),
                EndTime = new DateTime(2026, 7, 14, 9, 0, 0)
            }
        ];

        _view.FilterDateToReturn = new DateTime(2026, 7, 14);
        _view.FilterOptionsToReturn.Enqueue(FilterOption.Day);
        _service.FilterExceptionToThrow = new InvalidOperationException("filter failed");

        _controller.ViewSessions();

        Assert.Multiple(() =>
        {
            Assert.That(_view.LastError, Is.EqualTo("Unexpected error: filter failed"));
            Assert.That(_view.DisplaySessionsCalls, Is.EqualTo(1));
            Assert.That(_view.DisplayFilterMenuCalls, Is.EqualTo(1));
            Assert.That(_service.DayArgument, Is.Null);
        });
    }
}
