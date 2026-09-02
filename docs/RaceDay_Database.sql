/*
    RaceDay Event Management System
    PROG6212 Programming 2B - Part 1
    SQL Server / SSMS

    The schema below is designed to match RaceDay_ERD.pdf.
*/

IF DB_ID('RaceDayDB') IS NULL
BEGIN
    CREATE DATABASE RaceDayDB;
END
GO

USE RaceDayDB;
GO

-- Drop tables in dependency order so the script can be re-run cleanly.
IF OBJECT_ID('dbo.Results', 'U') IS NOT NULL DROP TABLE dbo.Results;
IF OBJECT_ID('dbo.Enrolments', 'U') IS NOT NULL DROP TABLE dbo.Enrolments;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Routes', 'U') IS NOT NULL DROP TABLE dbo.Routes;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users
(
    UserID INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(20) NULL,
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Organiser', 'Participant'))
);
GO

CREATE TABLE dbo.Routes
(
    RouteID INT IDENTITY(1,1) CONSTRAINT PK_Routes PRIMARY KEY,
    RouteName NVARCHAR(100) NOT NULL,
    StartPoint NVARCHAR(150) NOT NULL,
    EndPoint NVARCHAR(150) NOT NULL,
    ElevationGain DECIMAL(8,2) NULL,
    RouteMapURL NVARCHAR(500) NULL
);
GO

CREATE TABLE dbo.Events
(
    EventID INT IDENTITY(1,1) CONSTRAINT PK_Events PRIMARY KEY,
    OrganiserID INT NOT NULL,
    RouteID INT NULL,
    EventName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(150) NOT NULL,
    Distance DECIMAL(6,2) NOT NULL,
    EventType NVARCHAR(10) NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Events_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Events_Organiser
        FOREIGN KEY (OrganiserID) REFERENCES dbo.Users(UserID),
    CONSTRAINT FK_Events_Route
        FOREIGN KEY (RouteID) REFERENCES dbo.Routes(RouteID),
    CONSTRAINT CK_Events_EventType
        CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    CONSTRAINT CK_Events_Distance
        CHECK (Distance > 0)
);
GO

CREATE TABLE dbo.Categories
(
    CategoryID INT IDENTITY(1,1) CONSTRAINT PK_Categories PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName NVARCHAR(80) NOT NULL,
    MinimumAge INT NULL,
    MaximumAge INT NULL,
    CategoryDistance DECIMAL(6,2) NULL,

    CONSTRAINT FK_Categories_Event
        FOREIGN KEY (EventID) REFERENCES dbo.Events(EventID),
    CONSTRAINT CK_Categories_AgeRange
        CHECK (MinimumAge IS NULL OR MaximumAge IS NULL OR MinimumAge <= MaximumAge),
    CONSTRAINT CK_Categories_Age
        CHECK (MinimumAge IS NULL OR MinimumAge >= 0),
    CONSTRAINT CK_Categories_MaxAge
        CHECK (MaximumAge IS NULL OR MaximumAge >= 0),
    CONSTRAINT CK_Categories_Distance
        CHECK (CategoryDistance IS NULL OR CategoryDistance > 0),
    CONSTRAINT UQ_Categories_Event_Name
        UNIQUE (EventID, CategoryName)
);
GO

CREATE TABLE dbo.Enrolments
(
    EnrolmentID INT IDENTITY(1,1) CONSTRAINT PK_Enrolments PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL CONSTRAINT DF_Enrolments_EnrolmentDate DEFAULT SYSDATETIME(),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Enrolments_Status DEFAULT 'Confirmed',

    CONSTRAINT FK_Enrolments_Participant
        FOREIGN KEY (ParticipantID) REFERENCES dbo.Users(UserID),
    CONSTRAINT FK_Enrolments_Event
        FOREIGN KEY (EventID) REFERENCES dbo.Events(EventID),
    CONSTRAINT FK_Enrolments_Category
        FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(CategoryID),
    CONSTRAINT CK_Enrolments_Status
        CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    CONSTRAINT UQ_Enrolments_Participant_Event
        UNIQUE (ParticipantID, EventID)
);
GO

CREATE TABLE dbo.Results
(
    ResultID INT IDENTITY(1,1) CONSTRAINT PK_Results PRIMARY KEY,
    EnrolmentID INT NOT NULL CONSTRAINT UQ_Results_Enrolment UNIQUE,
    FinishTime TIME NULL,
    FinishingPosition INT NULL,
    RecordedAt DATETIME2 NOT NULL CONSTRAINT DF_Results_RecordedAt DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Results_Enrolment
        FOREIGN KEY (EnrolmentID) REFERENCES dbo.Enrolments(EnrolmentID),
    CONSTRAINT CK_Results_Position
        CHECK (FinishingPosition IS NULL OR FinishingPosition > 0)
);
GO

/* =========================
   Seed data
   Minimum required:
   2 Organisers
   2 Participants
   3 Events
   Categories for each event
   Sample enrolments
   ========================= */

-- PasswordHash values are placeholders for seeded accounts.
-- Part 2 must hash passwords rather than store original passwords.
INSERT INTO dbo.Users
    (FirstName, LastName, Email, PasswordHash, Role, Phone)
VALUES
    ('Lerato', 'Mokoena', 'lerato.mokoena@raceday.co.za', 'SEEDED_HASH_ORGANISER_01', 'Organiser', '0825551001'),
    ('Thabo', 'Nkosi', 'thabo.nkosi@raceday.co.za', 'SEEDED_HASH_ORGANISER_02', 'Organiser', '0835551002'),
    ('Ayanda', 'Dlamini', 'ayanda.dlamini@example.co.za', 'SEEDED_HASH_PARTICIPANT_01', 'Participant', '0845552001'),
    ('Sipho', 'Mthembu', 'sipho.mthembu@example.co.za', 'SEEDED_HASH_PARTICIPANT_02', 'Participant', '0855552002');
GO

INSERT INTO dbo.Routes
    (RouteName, StartPoint, EndPoint, ElevationGain, RouteMapURL)
VALUES
    ('Johannesburg City 10K Route', 'Mary Fitzgerald Square', 'Mary Fitzgerald Square', 85.00, 'https://example.com/routes/jhb-10k'),
    ('Soweto Community 21K Route', 'FNB Stadium', 'FNB Stadium', 210.00, 'https://example.com/routes/soweto-21k'),
    ('Cape Town Coastal Cycle Route', 'Cape Town Stadium', 'Cape Town Stadium', 620.00, 'https://example.com/routes/cape-town-cycle');
GO

INSERT INTO dbo.Events
    (OrganiserID, RouteID, EventName, Description, EventDate, Location, Distance, EventType)
VALUES
    (1, 1, 'Johannesburg City 10K', 'A city road running event suitable for recreational and competitive runners.', '2026-10-18', 'Johannesburg, Gauteng', 10.00, 'Run'),
    (1, 2, 'Soweto Community 21K', 'A community half-marathon celebrating running and community participation.', '2026-11-08', 'Soweto, Gauteng', 21.10, 'Run'),
    (2, 3, 'Cape Town Coastal Cycle', 'A road cycling event along selected coastal roads around Cape Town.', '2027-01-24', 'Cape Town, Western Cape', 60.00, 'Cycle');
GO

INSERT INTO dbo.Categories
    (EventID, CategoryName, MinimumAge, MaximumAge, CategoryDistance)
VALUES
    (1, 'Junior Under 20', 13, 19, 10.00),
    (1, 'Senior', 20, 39, 10.00),
    (1, 'Masters 40+', 40, NULL, 10.00),
    (2, 'Junior Under 20', 16, 19, 21.10),
    (2, 'Senior', 20, 39, 21.10),
    (2, 'Masters 40+', 40, NULL, 21.10),
    (3, 'Open Cycle', 16, NULL, 60.00),
    (3, 'Masters Cycle 40+', 40, NULL, 60.00);
GO

INSERT INTO dbo.Enrolments
    (ParticipantID, EventID, CategoryID, Status)
VALUES
    (3, 1, 2, 'Confirmed'),
    (4, 1, 2, 'Confirmed'),
    (3, 2, 5, 'Confirmed'),
    (4, 2, 5, 'Pending'),
    (3, 3, 7, 'Confirmed');
GO

INSERT INTO dbo.Results
    (EnrolmentID, FinishTime, FinishingPosition)
VALUES
    (1, '00:52:18', 47),
    (2, '00:58:42', 91);
GO

-- Verification queries for the live SSMS demonstration.
SELECT * FROM dbo.Users;
SELECT * FROM dbo.Routes;
SELECT * FROM dbo.Events;
SELECT * FROM dbo.Categories;
SELECT * FROM dbo.Enrolments;
SELECT * FROM dbo.Results;
GO
