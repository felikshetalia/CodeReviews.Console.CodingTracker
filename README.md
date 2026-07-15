# Coding Tracker App

Console-based CRUD application for tracking coding sessions. Built with C#/.NET, SQLite, Dapper, and Spectre.Console.

The project is based on [The C# Academy Coding Tracker challenge](https://www.thecsharpacademy.com/project/13/coding-tracker) and builds on the concepts I previously practised in my [Habit Tracker project](https://github.com/felikshetalia/CodeReviews.Console.HabitTracker).

# Given Requirements

- Users must be able to create, view, update, and delete coding sessions.
- Each coding session must contain an ID, start time, end time, and duration.
- Users enter the start and end times, while duration is calculated automatically.
- Date and time input must follow a clearly specified format and be validated.
- The application must store its data in a SQLite database.
- The database and the required table must be created automatically when the application starts.
- The database connection string must be stored in `appsettings.json`.
- Dapper must be used for database access.
- Spectre.Console must be used to display the user interface.
- The application must follow separation of concerns and keep classes in separate files.
- Repeated code should be reduced by following the DRY principle.
- Database records must be converted into `CodingSession` objects rather than being exposed to the rest of the application as anonymous data.
- The project should include a README explaining the implementation and thought process.

# Features

- **SQLite database initialization**
  - The application creates the local database and `CodingSessions` table when they do not already exist.
  - The database path is loaded from `Configuration/appsettings.json`.
  - A connection factory centralizes the creation of SQLite connections.

- **Coding-session CRUD operations**
  - Add coding sessions.
  - View all coding sessions.
  - Update an existing session by ID.
  - Delete an existing session by ID.
  - Sessions are ordered from newest to oldest.

- **Two ways to add a coding session**
  - Enter a past session manually by providing its start and end times.
  - Start a stopwatch and stop it when the coding session finishes.

- **Automatic duration calculation**
  - Users never enter duration manually.
  - `Duration` is calculated from `EndTime - StartTime`, which prevents conflicting values from being stored.

- **Filtering**
  - Show all sessions.
  - Filter by a specific day.
  - Filter by week.
  - Filter by month and year.
  - Filter by year.
  - Date-range queries use an inclusive start and exclusive end boundary.

- **Input validation**
  - Dates and times must match the expected format.
  - The end time must be later than the start time.
  - Session IDs must be positive.
  - Update and delete workflows display the existing sessions before asking the user to choose an ID.
  - Missing sessions and invalid operations are reported without terminating the application.

- **Console interface**
  - Spectre.Console selection menus are used instead of numeric menu commands.
  - Coding sessions are presented in formatted tables.
  - Error and success messages are displayed consistently.

# Architecture

The project uses an MVC-inspired layered structure with interfaces between the main dependencies:

```text
Program
├── AppController
│   ├── IAppView
│   └── ICodingSessionController
│
└── CodingSessionController
    ├── ICodingSessionView
    └── ICodingSessionService
        └── CodingSessionService
            └── ICodingSessionRepo
                └── CodingSessionRepo
                    └── IDatabaseConnectionFactory
                        └── SQLiteConnectionFactory
```

The main responsibilities are:

- **Views** handle Spectre.Console input and output.
- **Controllers** coordinate menu flows and connect views with services.
- **Services** contain validation, use-case logic, and date-range calculations.
- **Repositories** contain Dapper queries and database mapping.
- **Database classes** create connections and initialize the schema.
- **Models** represent coding sessions in the application.
- **Program.cs** acts as the composition root and connects concrete implementations through their interfaces.

# Design Decisions

- I kept `CodingSession.StartTime` and `CodingSession.EndTime` as `DateTime` values instead of changing the domain model to match SQLite's text representation.
- Conversion between `DateTime` and the database date format is handled at the persistence boundary.
- Duration is a calculated property rather than a separate database column.
- Filtering is placed inside the **View sessions** workflow instead of adding several extra options to the main menu.
- Week filtering accepts one date and calculates the Monday-to-Monday range in the service.
- I used a coding-session-specific repository interface instead of introducing a generic repository before another real repository exists.
- Interfaces are used at meaningful dependency boundaries rather than creating one interface for every class.
- Dependencies are constructed manually in `Program.cs` instead of introducing a dependency-injection framework for a small console application.

# Technologies Used

- C#
- .NET 9
- SQLite
- Dapper
- Microsoft.Data.Sqlite
- Spectre.Console
- JSON configuration through `appsettings.json`
- NUnit 3

# Tests

The solution contains both unit tests and database integration tests.

- **Database initialization tests**
  - Verify that the database and required table are created.

- **Repository integration tests**
  - Use isolated SQLite test databases.
  - Cover create, read, update, and delete behavior.
  - Verify that update and delete affect only the selected record.
  - Test ordering and generated IDs.
  - Test day, date-range, month, and year filters.
  - Test inclusive and exclusive filter boundaries.

- **Service unit tests**
  - Use a test double implementing `ICodingSessionRepo`.
  - Verify validation, invalid-ID short-circuiting, update behavior, repository delegation, and week-boundary calculations.

- **Controller unit tests**
  - Use fake services, views, and controllers.
  - Verify menu routing, workflow branching, displayed messages, input order, filtering behavior, and error handling.

Run the tests with:

```bash
dotnet test CodingTracker/CodingTracker.sln
```

# Running the Application

1. Clone the repository:

```bash
git clone https://github.com/felikshetalia/CodeReviews.Console.CodingTracker.git
```

2. Move into the repository:

```bash
cd CodeReviews.Console.CodingTracker
```

3. Restore the dependencies:

```bash
dotnet restore CodingTracker/CodingTracker.sln
```

4. Run the application:

```bash
dotnet run --project CodingTracker/CodingTracker.csproj
```

# What I've Learned from This Project

- Using Dapper with SQLite for parameterized `SELECT`, `INSERT`, `UPDATE`, and `DELETE` operations.
- Creating a local SQLite database and schema automatically when the program starts.
- Loading connection settings from `appsettings.json`.
- Using a factory to isolate database-connection creation.
- Separating console presentation, application flow, business logic, and persistence.
- Applying interfaces and constructor injection to make classes replaceable and testable.
- Avoiding speculative base classes and generic abstractions until there is real duplication.
- Parsing and formatting dates consistently across console input and database storage.
- Calculating day, week, month, and year ranges using inclusive lower and exclusive upper boundaries.
- Building reusable Spectre.Console prompts and tables.
- Writing parameterized NUnit tests with `TestCase` and `TestCaseSource`.
- Distinguishing unit tests from repository integration tests.
- Creating handwritten test doubles that record calls and return configured results.
- Testing not only final results, but also whether dependencies were called with the correct arguments and in the correct order.

# Areas to Improve

- Inject a clock or `TimeProvider` so stopwatch tests do not depend on the system clock.
- Improve stopwatch recovery so an active timed session is not lost if the application closes unexpectedly.
- Make database error handling more consistent and avoid leaking SQLite-specific exceptions into higher layers.
- Reduce repeated database-row mapping and date-parsing code in the repository.
- Consider accepting seconds as optional manual input while preserving them for stopwatch sessions.
- Add automated test coverage reporting.
- Add screenshots or a short demonstration of the console interface.
