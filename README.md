# BLA App

### Full-Stack Task Management Application

A full-stack task management application developed as a **.NET Technical Interview Exercise**, using modern software architecture and development practices.

The application provides **user authentication, task management, business-rule enforcement, task history, and a responsive Angular frontend**.

---

##  Tech Stack

| Layer               | Technology                         |
| ------------------- | ---------------------------------- |
| Backend             | **.NET 8 / ASP.NET Core Web API**  |
| Architecture        | **Clean Architecture**             |
| Application Pattern | **CQRS + MediatR**                 |
| ORM                 | **Entity Framework Core**          |
| Authentication      | **ASP.NET Core Identity + JWT**    |
| Functional Results  | **CSharpFunctionalExtensions**     |
| Database            | **SQL Server / EF Core**           |
| Frontend            | **Angular 21**                     |
| UI                  | **Bootstrap**                      |
| Notifications       | **SweetAlert**                     |
| Forms               | **Angular Reactive Forms**         |
| Testing             | **xUnit + Moq + FluentAssertions** |
| API Documentation   | **Swagger / OpenAPI**              |

---

#  Project Overview

BLA App is a task management system where authenticated users can create and manage their own tasks.

The application was designed following **Clean Architecture**, **CQRS**, **Domain-Driven Design principles**, and **Tell, Don't Ask**.

### Main capabilities

* User registration
* User login
* JWT authentication
* Protected API endpoints
* Create tasks
* List tasks
* View task details
* Edit tasks
* Delete tasks
* Start tasks
* Complete tasks
* Cancel tasks
* View task history
* Unit tests
* Swagger documentation
* Angular 21 frontend
* Responsive UI

---

# Architecture

The backend follows **Clean Architecture** principles.
```
                     ┌──────────────────────┐
                     │         API          │
                     │   Controllers / JWT  │
                     └──────────┬───────────┘
                                │
                                ▼
                     ┌──────────────────────┐
                     │     Application      │
                     │ CQRS / MediatR / DTOs│
                     └──────────┬───────────┘
                                │
                                ▼
                     ┌──────────────────────┐
                     │        Domain        │
                     │ Entities / Rules     │
                     └──────────────────────┘
                                ▲
                                │
                     ┌──────────┴───────────┐
                     │    Infrastructure    │
                     │ EF Core / Repository │
                     └──────────────────────┘
```

### Dependency direction

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application
 ↓
Domain
```

The **Domain layer has no dependency on Infrastructure or API**.

---

# 📁 Solution Structure

```text
BlaApp
│
├── Api
│   ├── Controllers
│   ├── Middleware
│   ├── Extensions
│   ├── Program.cs
│   └── appsettings.json
│
├── Application
│   ├── Commands
│   │   ├── Authentication
│   │   └── Tasks
│   │
│   ├── Queries
│   │   ├── Authentication
│   │   └── Tasks
│   │
│   ├── DTOs
│   ├── Interfaces
│   └── Behaviors
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Rules
│
├── Infrastructure
│   ├── Persistence
│   ├── Repositories
│   ├── Identity
│   └── Authentication
│
├── Tests
│   ├── Application
│   ├── Infrastructure
│   └── Api
│
└── BlaFront
    ├── src
    ├── angular.json
    ├── package.json
    └── tsconfig.json
```

---

# 🧠 Domain Design

The domain layer contains the application's business rules.

Entities are intentionally protected from direct modification.

Properties use private setters:

```csharp
public Guid Id { get; private set; }

public Guid UserId { get; private set; }

public string Title { get; private set; }

public string Description { get; private set; }

public TaskStatus Status { get; private set; }

public DateTime DueDate { get; private set; }
```

The entity constructor is private.

Objects must be created through a factory method:

```csharp
public static Result<TaskItem> Create(
    Guid userId,
    string title,
    string description,
    DateTime dueDate)
```

This ensures that business rules are validated before an entity can exist in a valid state.

---

# 🔄 Task Lifecycle

Tasks have four possible states:

```csharp
public enum TaskStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}
```

### State transition

```text
                    ┌──────────────┐
                    │   Pending    │
                    └──────┬───────┘
                           │
                         Start
                           │
                           ▼
                    ┌──────────────┐
                    │  InProgress  │
                    └──────┬───────┘
                           │
                       Complete
                           │
                           ▼
                    ┌──────────────┐
                    │  Completed   │
                    └──────────────┘


Pending ───── Cancel ─────► Cancelled

InProgress ── Cancel ─────► Cancelled
```

Invalid transitions are rejected by the Domain layer.

For example:

```text
Completed → Start       ❌
Completed → Complete    ❌
Completed → Cancel      ❌

Cancelled → Start       ❌
Cancelled → Complete    ❌
Cancelled → Cancel      ❌
```

---

# 📣 Tell, Don't Ask

The domain entity exposes behavior instead of allowing external layers to manipulate its state directly.

For example:

```csharp
public Result Start()
```

```csharp
public Result Complete()
```

```csharp
public Result Cancel()
```

Instead of:

```csharp
task.Status = TaskStatus.Completed;
```

the application tells the entity what should happen:

```csharp
task.Complete();
```

This keeps business rules inside the Domain layer.

---

# 🔀 CQRS

The application uses **CQRS (Command Query Responsibility Segregation)**.

Commands modify state.

Queries retrieve data.

### Commands

```text
CreateTaskCommand
UpdateTaskCommand
DeleteTaskCommand
StartTaskCommand
CompleteTaskCommand
CancelTaskCommand

RegisterUserCommand
LoginCommand
```

### Queries

```text
GetTasksQuery
GetTaskByIdQuery
GetTaskHistoryQuery
```

MediatR is used to dispatch Commands and Queries to their respective handlers.

```text
Controller
    │
    ▼
 IMediator
    │
    ├───────────────► Command ──► Handler
    │
    └───────────────► Query ────► Handler
```

---

# 👤 Current User

The application does not trust a `UserId` sent by the frontend.

Instead, the authenticated user is obtained through:

```csharp
ICurrentUser
```

For example, the Create Task handler uses:

```csharp
_currentUser.UserId
```

This provides an additional security boundary and ensures that users operate on their own data.

---

# 🔐 Authentication

Authentication uses:

* ASP.NET Core Identity
* JWT Bearer Authentication
* ASP.NET Core Authorization

### Authentication flow

```text
User
 │
 ▼
Login
 │
 ▼
ASP.NET Identity
 │
 ▼
JWT Token
 │
 ▼
Angular
 │
 ▼
Authorization Header
 │
 ▼
ASP.NET Core
 │
 ▼
[Authorize]
 │
 ▼
Protected Endpoint
```

Example HTTP header:

```http
Authorization: Bearer <JWT_TOKEN>
```

---

# 🌐 API

The API exposes endpoints for authentication and task management.

### Authentication

```text
POST /api/auth/register
POST /api/auth/login
```

### Tasks

```text
POST   /api/tasks
GET    /api/tasks
GET    /api/tasks/{id}
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}

POST   /api/tasks/{id}/start
POST   /api/tasks/{id}/complete
POST   /api/tasks/{id}/cancel

GET    /api/tasks/{id}/history
```

Task endpoints require authentication.

---

# 🖥️ Frontend

The frontend is built with **Angular 21**.

Feature-based organization is used:

```text
src/app
│
├── core
│   ├── guards
│   ├── interceptors
│   └── services
│
├── features
│   │
│   ├── auth
│   │   ├── login
│   │   └── register
│   │
│   ├── tasks
│   │   ├── list
│   │   ├── create
│   │   ├── detail
│   │   └── edit
│   │
│   └── user
│
├── nav
│
└── app.routes.ts
```

---

# 🧩 Angular Services

### AuthService

Responsible for:

* Login
* Registration
* JWT storage
* Authentication state
* Logout

### TaskService

Responsible for:

* Task CRUD
* Start
* Complete
* Cancel
* Task history

### UserService

Responsible for user-related API operations.

---

# 🛡️ Route Guard

The Angular application uses an authentication guard to protect task routes.

Unauthenticated users cannot access:

```text
/tasks
/tasks/create
/tasks/:id
/tasks/:id/edit
```

The navigation menu provides:

```text
User
Tasks
Logout
```

The **Tasks** section is only available to authenticated users.

---

# 📝 Reactive Forms

All application forms use Angular Reactive Forms.

Example:

```typescript
form = this.fb.group({
  title: ['', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(100)
  ]],

  description: ['', [
    Validators.required,
    Validators.maxLength(500)
  ]],

  dueDate: ['', Validators.required],
});
```

Angular's `inject()` API is used for dependency injection where appropriate:

```typescript
private fb = inject(FormBuilder);
```

---

# 📊 Task List

The task list uses a Bootstrap table.

The available actions depend on the current task status.

| Status        | Available Actions                                    |
| ------------- | ---------------------------------------------------- |
| 🟡 Pending    | Detail · Edit · Delete · Start · Cancel · History    |
| 🔵 InProgress | Detail · Edit · Delete · Complete · Cancel · History |
| 🟢 Completed  | Detail · History                                     |
| 🔴 Cancelled  | Detail · History                                     |

The frontend controls which buttons are displayed, but the **backend remains responsible for enforcing business rules**.

---

# 📜 Task History

Every relevant task state transition creates a history entry.

The history records information such as:

* Task ID
* Previous status
* New status
* Date/time
* User responsible for the change

The Angular frontend displays the history in a Bootstrap modal.

```text
Task List
    │
    └── History
          │
          ▼
   GetTaskHistoryQuery
          │
          ▼
   GetTaskHistoryHandler
          │
          ▼
       EF Core
          │
          ▼
   Task History Records
```

---

# 🧪 Testing

The project uses:

* **xUnit**
* **Moq**
* **FluentAssertions**
* **EF Core InMemory**

Tests are separated according to application layers.

```text
Tests
│
├── Domain
│   └── TaskItemTests
│
├── Application
│   └── Commands
│       └── Tasks
│           └── CreateTask
│               └── CreateTaskCommandHandlerTests
│
├── Infrastructure
│   └── Persistence
│       └── Repositories
│           └── TaskRepositoryTests
│
└── Api
    └── Controllers
        └── TasksControllerTests
```

Run all tests:

```bash
dotnet test
```

---

# 🧪 Create Task Test Strategy

The Create Task use case is tested across multiple layers.

### Domain

Tests business rules and entity creation.

### Command Handler

Tests:

* Current user retrieval.
* Domain factory method.
* Business-rule failures.
* Repository invocation.
* Returned task ID.

Dependencies are mocked with Moq.

### Repository

Tests:

* EF Core `DbSet`.
* `AddAsync`.
* `SaveChangesAsync`.
* Actual persistence using EF Core InMemory.

### Controller

Tests:

* Command creation.
* MediatR invocation.
* Successful `CreatedAtAction` response.
* `BadRequest` response when the command fails.

This provides isolation between layers while validating each responsibility independently.

---

# 🤖 Generative AI

The technical assessment specifically requested the use of a Generative AI coding tool.

AI was used as a development assistant to generate initial scaffolding, implementation ideas, code samples, and test structures.

A representative prompt used during development was:

```text
Generate a RESTful task management API using .NET 8, ASP.NET Core Web API,
Entity Framework Core and Clean Architecture.

The application must support:

- User registration.
- User login.
- JWT authentication.
- Authorized and non-authorized endpoints.
- CRUD operations for tasks.
- Tasks associated with the authenticated user.
- Task fields: Id, Title, Description, Status and DueDate.
- Task status values: Pending, InProgress, Completed and Cancelled.

Use Clean Architecture with:
- Domain
- Application
- Infrastructure
- API

Use CQRS with MediatR:
- Commands for state-changing operations.
- Queries for read operations.

Use Entity Framework Core for persistence.

Entities must use private constructors and factory methods returning
CSharpFunctionalExtensions Result<T>.

Entity properties should use private setters.

Business operations should be implemented as methods on the domain entities
following the Tell, Don't Ask principle.

Use ICurrentUser to retrieve the authenticated user's ID instead of trusting
a user ID supplied by the client.

Use ASP.NET Core Identity and JWT for authentication.

Use repositories and an IApplicationDbContext abstraction.

Use xUnit, Moq and FluentAssertions for unit tests.

Include tests for:
- Domain rules.
- Command handlers.
- Repository operations.
- API endpoints.

Also provide Swagger configuration for JWT authentication.
```

---

# 🔍 How AI Suggestions Were Validated

AI-generated code was not accepted blindly.

The suggestions were reviewed against:

### Architecture

* Dependency direction.
* Separation of concerns.
* Domain isolation.
* Repository abstraction.
* CQRS boundaries.

### Business Rules

Task state transitions were manually reviewed to ensure invalid transitions were rejected.

### Authentication

JWT configuration and authorization were manually validated.

Special attention was given to ensuring that the authenticated user's identity comes from the JWT through `ICurrentUser`.

### Data Access

EF Core mappings and repository behavior were tested independently.

### Unit Tests

Generated tests were reviewed and modified to match the actual application contracts and implementation.

---

# 🛠️ Improvements Made to AI Output

The AI-generated implementation was adapted to the project's design decisions.

Examples include:

* Replacing public entity setters with private setters.
* Adding private constructors.
* Implementing Factory Methods returning `Result<T>`.
* Moving business rules into Domain entities.
* Introducing CQRS with MediatR.
* Using `Maybe<T>` for nullable query results.
* Introducing `ICurrentUser`.
* Separating Commands and Queries.
* Adding task history.
* Adding tests for individual layers.
* Adding Angular route guards.
* Using Reactive Forms.
* Integrating Bootstrap and SweetAlert.
* Ensuring frontend actions reflect backend business rules.

---

# ⚠️ Edge Cases

The application considers several edge cases.

### Authentication

Unauthenticated users cannot access protected task endpoints.

### Authorization

A user should only be able to access and modify their own tasks.

### Invalid State Transitions

Invalid task transitions are rejected by the Domain layer.

### Invalid Creation

Invalid task data is rejected by the Factory Method.

### Missing Tasks

Queries that may not find a task use:

```csharp
Maybe<T>
```

instead of relying on nullable return values.

---

# ⚙️ Prerequisites

Before running the application, make sure the following are installed:

* .NET 8 SDK
* Node.js
* npm
* Angular CLI 21
* SQL Server or another compatible configured database

Verify .NET:

```bash
dotnet --version
```

Verify Node:

```bash
node --version
```

Verify npm:

```bash
npm --version
```

Verify Angular:

```bash
ng version
```

---

# 🚀 Backend Setup

## 1. Configure the database connection

Open:

```text
Api/appsettings.Development.json
```

Locate the connection string with the key:

```text
DbConnection
```

Example:

```json
{
  "ConnectionStrings": {
    "DbConnection": "Server=localhost;Database=BlaAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace the connection string with the connection string required by your local database server.

> ⚠️ Do not commit passwords, secrets, or sensitive connection strings to source control.

---

# 🗄️ Database Setup

After configuring `DbConnection`, execute the Entity Framework Core migrations.

From the solution directory:

```bash
dotnet ef database update
```

This will apply the existing migrations and create/update the database in the configured database server.

If the EF CLI tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

If the solution requires specifying the projects explicitly:

```bash
dotnet ef database update --project Infrastructure --startup-project Api
```

---

# 🌐 Frontend Setup

The Angular application is located inside:

```text
BlaApp/BlaFront
```

Navigate to the frontend directory:

```bash
cd BlaFront
```

Install all Angular 21 dependencies:

```bash
npm i
```

This installs the dependencies defined in `package.json`.

---

# ▶️ Running the Application

## Backend

Run the ASP.NET Core API from Visual Studio or execute:

```bash
dotnet run
```

---

## Frontend

From:

```text
BlaApp/BlaFront
```

run:

```bash
ng serve
```

or:

```bash
npm start
```

The Angular CLI will display the local URL where the frontend is running.

---

# 📖 Swagger

Swagger/OpenAPI is available from the backend when the application is running.

Swagger can be used to:

* View API endpoints.
* Register users.
* Login.
* Obtain a JWT.
* Authorize Swagger.
* Test protected endpoints.

---

# 🔑 Using JWT in Swagger

1. Start the API.
2. Open Swagger.
3. Register a user or use the seeded credentials.
4. Login.
5. Copy the returned JWT.
6. Click **Authorize**.
7. Enter:

```text
Bearer YOUR_JWT_TOKEN
```

8. Click **Authorize**.
9. Protected endpoints can now be executed from Swagger.

---

# 👤 Demo Credentials

The application includes seeded/demo data and credentials for demonstration purposes.

Use the credentials configured in the application's seed configuration to test the authentication flow.

If the seed credentials are changed, the credentials defined by the current seed configuration should be used.

---

# 📦 Database Migrations

Existing EF Core migrations are included in the project.

To apply them:

```bash
dotnet ef database update
```

The migration process creates the required database objects for:

* Users
* Tasks
* Task history
* ASP.NET Identity tables
* Related entities

---

# 🔄 Complete Setup Flow

For a new environment, the recommended setup sequence is:

```text
1. Clone repository
        │
        ▼
2. Configure DbConnection
        │
        ▼
3. Run EF migrations
        │
        ▼
4. Start .NET API
        │
        ▼
5. Navigate to BlaFront
        │
        ▼
6. Run npm i
        │
        ▼
7. Run ng serve
        │
        ▼
8. Register/Login
        │
        ▼
9. Obtain JWT
        │
        ▼
10. Manage Tasks
```

---

# 🧹 Recommended Development Commands

### Restore backend dependencies

```bash
dotnet restore
```

### Build solution

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

### Update database

```bash
dotnet ef database update
```

### Install Angular dependencies

```bash
npm i
```

### Run Angular

```bash
ng serve
```

---

# 🔮 Future Improvements

Potential improvements include:

* Integration tests using `WebApplicationFactory`.
* Real database integration tests using Testcontainers.
* Pagination.
* Filtering and sorting.
* Refresh tokens.
* Role-based authorization.
* Global MediatR validation behavior.
* Centralized exception handling.
* CI/CD pipeline.
* Docker Compose.
* End-to-end tests.
* Advanced frontend state management.
* Improved audit logging.

---

# 👨‍💻 Design Principles

This project intentionally applies several software engineering principles:

### Clean Architecture

Business logic is independent from infrastructure and presentation.

### CQRS

Commands and Queries have separate responsibilities.

### Domain-Driven Design

Business rules are encapsulated inside Domain entities.

### Tell, Don't Ask

Entities expose behavior instead of allowing external code to manipulate their state.

### Dependency Inversion

Application logic depends on abstractions rather than concrete infrastructure implementations.

### Explicit Result Handling

Expected business failures are represented using:

```csharp
Result<T>
```

instead of exceptions.

Potentially missing values are represented using:

```csharp
Maybe<T>
```

---

# 📌 Summary

BLA App demonstrates a complete full-stack implementation combining:

```text
.NET 8
   +
Clean Architecture
   +
CQRS
   +
MediatR
   +
Entity Framework Core
   +
ASP.NET Identity
   +
JWT
   +
Angular 21
   +
Bootstrap
   +
Reactive Forms
   +
SweetAlert
   +
xUnit
   +
Moq
```

The project focuses not only on implementing functionality, but also on **separation of concerns, domain-driven business rules, testability, authentication, maintainability, and responsible use of Generative AI during development**.

