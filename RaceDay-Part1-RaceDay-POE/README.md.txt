# RaceDay – Event Management System

## 1. Project Overview

RaceDay is a web-based event management system developed for road running, walking and cycling events.

The system allows event organisers to manage events, categories, participant enrolments and race results. Participants can browse available events, enter events, view their enrolments and check their results.

The application was developed using ASP.NET Core MVC and ASP.NET Core Web API, with SQL Server for data storage and Azure Blob Storage for profile pictures.

## 2. User Roles

The system supports two user roles:

### Organiser
- View the organiser dashboard.
- Create, view, update and delete events.
- Manage event categories.
- View participant enrolments.
- Update enrolment statuses.
- Record and manage race results.

### Participant
- Register and log in.
- Browse and search upcoming events.
- View event details and categories.
- Enter available events.
- View personal enrolments.
- View race results.
- Update personal profile information.
- Upload and display a profile picture.

## 3. Technologies Used

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- Microsoft SQL Server
- HTML
- CSS
- JavaScript
- Bootstrap
- Azure Blob Storage
- Docker
- GitHub

## 4. Solution Structure

The solution contains the following projects:

- **RaceDay.Api:** Handles business logic, API endpoints, database communication and Azure Blob Storage.
- **RaceDay.Mvc:** Provides the web-based user interface.
- **RaceDay.Contracts:** Contains shared data transfer objects and contracts.
- **RaceDay.Api.Tests:** Contains API testing projects.

The MVC application communicates with the API through HTTP requests. The API handles communication with the SQL Server database.

## 5. Database Setup

The application uses Microsoft SQL Server.

Database name: `RaceDayDB_Part2`

To run the application:

1. Install Microsoft SQL Server.
2. Open SQL Server Management Studio.
3. Ensure the required RaceDay database and tables are available.
4. Configure the database connection string in the RaceDay.Api User Secrets or another secure configuration provider.
5. Do not commit passwords or database connection strings to GitHub.

## 6. Running the Application Locally

### Step 1: Run the API

Open the solution in Visual Studio.

Run the RaceDay.Api project.

The API should be available at:

`https://localhost:7251`

### Step 2: Run the MVC Application

Run the RaceDay.Mvc project.

The MVC application communicates with the API using the configured API base URL.

The application can then be accessed through its local development address.

## 7. Azure Blob Storage

Azure Blob Storage is used to store participant and organiser profile pictures.

The image upload process works as follows:

1. The user selects a profile picture through the MVC interface.
2. The MVC application sends the image to the API.
3. The API validates the uploaded file.
4. The API uploads the image to Azure Blob Storage.
5. The image URL is saved in the user's database record.
6. The MVC application retrieves and displays the profile picture through the API.

The Azure Storage connection string is stored securely using Visual Studio User Secrets.

The storage container is named `profile-pictures`.

## 8. Docker Setup

The RaceDay MVC application can be built and executed using Docker.

### Requirements
- Docker Desktop
- .NET 10 SDK

### Build the Docker Image

Open PowerShell in the main solution folder and execute:

`docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc:part3 .`

### Start the API

Run the API separately using:

`dotnet run --project RaceDay.Api --urls http://0.0.0.0:5000`

Keep the API running.

### Start the MVC Container

Open another PowerShell window and execute:

`docker run -d --name raceday-mvc -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:5000/ raceday-mvc:part3`

### Check the Container

Execute:

`docker ps`

To view container logs:

`docker logs raceday-mvc`

### Access the Application

Open the browser and visit:

`http://localhost:8080`

The MVC application communicates with the API running on the host computer.

## 9. Authentication and Security

The application supports registration and login for Organisers and Participants.

Role-based access restrictions prevent users from accessing functions that are not intended for their assigned roles.

Session management is used to maintain user login information.

Sensitive configuration information, including database and Azure Storage connection strings, must be stored securely and excluded from source control.

## 10. GitHub Repository

The project source code is maintained on GitHub.

Repository:

https://github.com/Tshepi-9/RaceDay-Part1

Part 3 branch:

`RaceDay-Part3`

GitHub is used for source control, version tracking and project development.

## 11. Screenshots and Demonstration Video

Screenshots of the completed application and a demonstration video will be included here.

The demonstration will cover:
- User registration and login.
- Organiser dashboard.
- Event and category management.
- Participant enrolments.
- Results management.
- Participant dashboard.
- Profile updates and picture uploads.
- Azure Blob Storage integration.
- Docker image creation and container execution.

## 12. Conclusion

RaceDay provides a centralised platform for managing running, walking and cycling events. It allows organisers to manage event information and results while giving participants access to event enrolments, personal profiles and race results.

The project demonstrates the use of ASP.NET Core MVC, Web API, SQL Server, Azure Blob Storage, Docker and GitHub.