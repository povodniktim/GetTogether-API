CREATE TABLE Users (
    ID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(255),
    Password VARCHAR(255),
    CreatedAt DATETIME NOT NULL,
    LastLoggedInAt DATETIME
);

CREATE TABLE Events (
    ID INT PRIMARY KEY,
    OrganizerID INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    Title VARCHAR(125) NOT NULL,
    Description TEXT NOT NULL,
    Date DATETIME NOT NULL,
    Location TEXT,
    MaxParticipants INT DEFAULT 5 NOT NULL,
    CONSTRAINT `Event_Organizer` FOREIGN KEY (OrganizerID) REFERENCES Users(ID)
);

CREATE TABLE EventParticipants (
    ID INT PRIMARY KEY,
    EventID INT NOT NULL,
    ParticipantID INT NOT NULL,
    CONSTRAINT `EventParticipants_Event` FOREIGN KEY (EventID) REFERENCES Events(ID),
    CONSTRAINT `EventParticipants_Participant` FOREIGN KEY (ParticipantID) REFERENCES Users(ID)
);

CREATE TABLE EventComments (
    ID INT PRIMARY KEY,
    EventID INT NOT NULL,
    UserID INT NOT NULL,
    Text TEXT,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT `EventComments_Event` FOREIGN KEY (EventID) REFERENCES Events(ID),
    CONSTRAINT `EventComments_User` FOREIGN KEY (UserID) REFERENCES Users(ID)
);

CREATE TABLE EventPhotos (
    ID INT PRIMARY KEY,
    EventID INT NOT NULL,
    URL VARCHAR(255) NOT NULL,
    UploadedAt DATETIME NOT NULL,
    CONSTRAINT `EventPhotos_Event` FOREIGN KEY (EventID) REFERENCES Events(ID)
);