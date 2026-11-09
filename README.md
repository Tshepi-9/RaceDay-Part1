# RaceDay – Event Management System

![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-blue)
![Docker](https://img.shields.io/badge/Docker-Enabled-blue)
![Azure](https://img.shields.io/badge/Azure-Blob%20Storage-blue)
![CI/CD](https://img.shields.io/badge/GitHub%20Actions-Passing-brightgreen)

## 1. Project Overview

RaceDay is a web-based event management system developed for road running, walking and cycling events.

The system provides a platform where event organisers can manage sporting events, categories, participant enrolments and race results. Participants can browse upcoming events, enter events, manage their profiles and view their results.

The application uses ASP.NET Core MVC for the user interface and ASP.NET Core Web API for backend operations. Microsoft SQL Server is used for data storage, while Azure Blob Storage handles profile picture uploads.

The project demonstrates the use of modern web development technologies, API integration, cloud storage, containerisation and automated testing.

## 2. Main Features

### Organiser Features

- Organiser registration and login.
- Organiser dashboard.
- Create, view, update and delete events.
- Manage event categories.
- View participant enrolments.
- Update enrolment statuses.
- Record participant race results.
- Manage event information.

### Participant Features

- Participant registration and login.
- Participant dashboard.
- Browse upcoming events.
- Search and filter events.
- View event details and categories.
- Enter available events.
- View personal event enrolments.
- View race results.
- Update personal profile information.
- Upload and display profile pictures.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Programming language |
| ASP.NET Core MVC | Frontend application |
| ASP.NET Core Web API | Backend services |
| .NET 10 | Application framework |
| Entity Framework Core | Database operations |
| Microsoft SQL Server | Database management |
| HTML | Webpage structure |
| CSS | Styling |
| JavaScript | Client-side functionality |
| Bootstrap | Responsive interface |
| Azure Blob Storage | Profile picture storage |
| Docker | Application containerisation |
| GitHub | Source control |
| GitHub Actions | Automated build and testing |

## 4. System Architecture

RaceDay uses a multi-project architecture.

### Project Structure

- **RaceDay.Mvc:** Handles the user interface and communicates with the API.
- **RaceDay.Api:** Handles backend operations, authentication, business logic, database communication and Azure Blob Storage.
- **RaceDay.Contracts:** Contains shared data transfer objects and contracts.
- **RaceDay.Api.Tests:** Contains automated API tests.

### Application Communication

The MVC application communicates with the Web API through HTTP requests.

The API handles database operations using Entity Framework Core and SQL Server.

Profile picture uploads follow this process:

1. The user selects a picture through the MVC application.
2. MVC sends the picture to the API.
3. The API validates the uploaded file.
4. The API uploads the picture to Azure Blob Storage.
5. The picture reference is saved in the database.
6. MVC retrieves and displays the picture through the API.

The MVC application does not connect directly to the database.

## 5. Database Configuration

RaceDay uses Microsoft SQL Server.

**Database name:** `RaceDayDB_Part2`

To configure the database:

1. Install Microsoft SQL Server.
2. Open SQL Server Management Studio.
3. Ensure the RaceDay database and required tables are available.
4. Configure the API database connection string.
5. Store sensitive connection details securely using Visual Studio User Secrets or another secure configuration provider.

Database passwords and sensitive connection strings must never be committed to GitHub.

## 6. Running the Application Locally

### Requirements

- Visual Studio 2022 or later.
- .NET 10 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio.
- Azure Storage account for profile picture functionality.

### Step 1: Clone the Repository

```bash
git clone https://github.com/Tshepi-9/RaceDay-Part1.git
```

### Step 2: Open the Solution

Open `RaceDay.Api.slnx` in Visual Studio.

### Step 3: Configure the API

Configure the database connection string and Azure Storage settings using User Secrets.

### Step 4: Run the API

Start the RaceDay.Api project.

### Step 5: Run the MVC Application

Start the RaceDay.Mvc project.

The MVC application communicates with the API using the configured API base URL.

## 7. Azure Blob Storage

Azure Blob Storage is used to store profile pictures.

The storage container used by the application is:

`profile-pictures`

The container is configured as private.

The API handles image validation, uploading and retrieval. MVC communicates with the API to display the stored images.

The Azure Storage connection string is stored securely using Visual Studio User Secrets.

Sensitive Azure credentials are excluded from the GitHub repository.

## 8. Docker Configuration

The RaceDay MVC application can be built and executed using Docker.

### Requirements

- Docker Desktop.
- .NET 10 SDK.
- RaceDay API configured and running.

### Step 1: Build the Docker Image

Open PowerShell in the main solution folder and run:

```powershell
docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc:part3 .
```

### Step 2: Start the API

Run the API separately:

```powershell
dotnet run --project RaceDay.Api --urls http://0.0.0.0:5000
```

Keep the API running.

### Step 3: Start the MVC Container

Open another PowerShell window and run:

```powershell
docker run -d --name raceday-mvc -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:5000/ raceday-mvc:part3
```

### Step 4: Verify the Container

```powershell
docker ps
```

To view the container logs:

```powershell
docker logs raceday-mvc
```

### Step 5: Access RaceDay

Open the browser and navigate to:

```text
http://localhost:8080
```

The MVC application runs inside the Docker container and communicates with the API running on the host computer.

## 9. Authentication and Security

RaceDay supports two user roles:

- Organiser
- Participant

The system uses registration, login and session management to maintain user authentication.

Role-based access restrictions ensure that users can only access functions permitted for their assigned roles.

Sensitive configuration information, including database passwords and Azure connection strings, is stored securely and excluded from source control.

## 10. GitHub Actions – CI/CD

GitHub Actions is used to automate the project's build and testing process.

The workflow is located at:

`.github/workflows/dotnet.yml`

The workflow performs the following tasks:

1. Checks out the GitHub repository.
2. Installs the .NET 10 SDK.
3. Restores project dependencies.
4. Builds the solution.
5. Runs automated tests.

The workflow runs when changes are pushed to the `RaceDay-Part3` branch or when a pull request targets that branch.

**Current status:** The GitHub Actions workflow has completed successfully.

## 11. GitHub Repository

**Repository:**

https://github.com/Tshepi-9/RaceDay-Part1/tree/RaceDay.POE.ST10491732/RaceDay-Part1-RaceDay-POE 

**Development branch:**

`RaceDay-Part3`

GitHub is used to maintain the project's source code, track changes and manage development commits.

## 12. Screenshots and Demonstration

Screenshots and a demonstration video will be included to show the completed system.
Screenshot of the CI workflow: 
 <img width="1357" height="639" alt="Screenshot 2026-11-09 192134" src="https://github.com/user-attachments/assets/61dfd077-c510-48de-9881-939ffe2ff571" />


The demonstration covers:
- Registration and login.
- Organiser dashboard.
- Event management.
- Category management.
- Participant enrolments.
- Results management.
- Participant dashboard.
- Event browsing and filtering.
- My Events and My Results.
- Profile management.
- Azure Blob Storage integration.
- Docker image creation and container execution.
- GitHub Actions CI/CD workflow.

The YouTube Video: https://youtu.be/cnEl58Z7yA4?si=74yx-wiMsCsFRSG1 

## 13. Conclusion

RaceDay provides a centralised event management platform for running, walking and cycling events.

The application allows organisers to manage sporting events and participant results while giving participants access to event information, enrolments, personal profiles and performance results.

The project demonstrates practical implementation of ASP.NET Core MVC, Web API, SQL Server, Azure Blob Storage, Docker and GitHub Actions.

It also demonstrates the importance of secure configuration, role-based access, API communication, automated testing and version control.
