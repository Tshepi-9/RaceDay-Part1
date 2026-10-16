# RaceDay API — Part 2

## 1. Project Overview

RaceDay is an event management system designed for running, walking and cycling events. The API allows Organisers to manage events and categories, while Participants can enrol in events and view their results.

This project implements the backend API for RaceDay Part 2.

## 2. User Roles

### Organiser

* Register and log in.
* View events.
* Create, update and delete their own events, subject to existing enrolments and categories.
* Manage categories for their own events.
* View enrolments for their own events.
* Update enrolment statuses.
* Record and view results for their own events.

### Participant

* Register and log in.
* View available events and categories.
* Enrol in events.
* View their own enrolments.
* View their own profile and results.

## 3. Technologies

* C#
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Swagger / OpenAPI
* Visual Studio
* GitHub Actions

## 4. Project Structure

* `RaceDay.Api` — API controllers, models, database context, filters and configuration.
* `RaceDay.Contracts` — shared contracts, if used by the solution.
* `RaceDay.Api.Tests` — automated tests.

## 5. Database

The application uses Entity Framework Core with SQL Server.

The main entities are:

* User
* Route
* Event
* Category
* Enrolment
* Result

Relationships between the entities are configured in `RaceDayDbContext`.

### Database Setup

1. Install the required .NET SDK and SQL Server.
2. Configure `DefaultConnection` in `appsettings.json`.
3. Open the Package Manager Console.
4. Select `RaceDay.Api` as the default project.
5. Apply the existing Entity Framework migration using `Update-Database`.

Do not commit real passwords, production connection strings or other secrets to the repository.

## 6. Authentication

The API provides the following authentication endpoints:

* `POST /api/auth/register`
* `POST /api/auth/login`
* `POST /api/auth/logout`

Passwords are hashed rather than stored as plain text.

The application uses ASP.NET Core session storage to track the logged-in user's ID and role.

## 7. API Endpoints

### Profile

* `GET /api/profile/me`
* `PUT /api/profile/me`

### Events

* `GET /api/events`
* `GET /api/events/{id}`
* `GET /api/events/mine`
* `POST /api/events`
* `PUT /api/events/{id}`
* `DELETE /api/events/{id}`

### Categories

* `GET /api/events/{eventId}/categories`
* `POST /api/events/{eventId}/categories`
* `PUT /api/categories/{id}`
* `DELETE /api/categories/{id}`

### Enrolments

* `POST /api/enrolments`
* `GET /api/enrolments/mine`
* `GET /api/enrolments/event/{eventId}`
* `PUT /api/enrolments/{id}/status`

### Results

* `POST /api/results`
* `GET /api/results/mine`
* `GET /api/results/event/{eventId}`

## 8. Role-Based Access Control

The API uses session information and role checks to restrict protected operations.

Organisers can manage their own events and related records. Participants can enrol in events and access their own enrolments and results.

An Organiser must not manage another Organiser's events, and a Participant must not use Organiser-only operations.

## 9. Running the API

1. Open `RaceDay.sln` in Visual Studio.
2. Confirm the SQL Server connection string.
3. Apply the database migration if necessary.
4. Set `RaceDay.Api` as the startup project.
5. Run the application.
6. Open the Swagger URL shown by Visual Studio.

## 10. Testing

Automated tests are located in `RaceDay.Api.Tests`.

Run the tests through Visual Studio Test Explorer or by using:

`dotnet test`

Test coverage should include authentication, event management, enrolments and role enforcement.

## 11. Continuous Integration

The GitHub Actions workflow is stored in:

`.github/workflows/dotnet.yml`

The workflow restores dependencies, builds the solution and runs the automated tests.

The workflow status should be checked in the repository's Actions tab.

<img width="1332" height="468" alt="image" src="https://github.com/user-attachments/assets/3b8252ee-9cda-4ef4-88c7-c59a7814dab2" />

## 12. Demonstration

Demonstration video: https://youtu.be/kPj1Cwwkh90?si=ssbnpEvQLwWZt5LZ 

The demonstration should show the project structure, database configuration, authentication, role restrictions, event creation, categories, enrolments, results, Swagger and automated test results.

## 13. Repository

GitHub repository: https://github.com/Tshepi-9/RaceDay-Part1/tree/RaceDay.Api-Part2
<img width="1332" height="468" alt="Screenshot 2026-10-17 004610" src="https://github.com/user-attachments/assets/5fe38bd5-738d-44a5-a4de-dae093656531" />
below is also a screenshot showing successful unit tests
<img width="784" height="400" alt="Screenshot 2026-10-17 013303" src="https://github.com/user-attachments/assets/f6c1f604-33c9-4327-8060-b71adfb3ceea" />
