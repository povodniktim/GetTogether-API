CREATE TABLE Users (
    ID INT PRIMARY KEY,
    firstName VARCHAR(50) NOT NULL,
    lastName VARCHAR(50) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    createdAt DATETIME NOT NULL,
    profileImageUrl TEXT,
    googleID VARCHAR(255),
    facebookID VARCHAR(255),
    twitterID VARCHAR(255),
    appleID VARCHAR(255)
);

CREATE TABLE Activities (
    ID INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE Events (
    ID INT PRIMARY KEY,
    organizerID INT NOT NULL,
    activityID INT NOT NULL,
    createdAt DATETIME NOT NULL,
    title VARCHAR(150) NOT NULL,
    description TEXT,
    date DATETIME NOT NULL,
    location VARCHAR(255),
    maxParticipants INT,
    visibility ENUM ('private', 'public') DEFAULT 'public',
    CONSTRAINT `Event_Organizer` FOREIGN KEY (organizerID) REFERENCES Users (ID),
    CONSTRAINT `Event_Activity` FOREIGN KEY (activityID) REFERENCES Activities (ID)
);

CREATE TABLE EventParticipants (
    ID INT PRIMARY KEY,
    participantID INT NOT NULL,
    eventID INT NOT NULL,
    status ENUM ('going', 'maybe', 'not going'),
    statusChangedAt DATETIME,
    CONSTRAINT `EventParticipants_Participant` FOREIGN KEY (participantID) REFERENCES Users (ID),
    CONSTRAINT `EventParticipants_Event` FOREIGN KEY (eventID) REFERENCES Events (ID)
);

CREATE TABLE Notifications (
    ID INT PRIMARY KEY,
    userID INT NOT NULL,
    eventID INT,
    participantID INT,
    status ENUM ('seen', 'not seen', 'deleted') DEFAULT 'not seen',
    CONSTRAINT `Notifications_User` FOREIGN KEY (userID) REFERENCES Users (ID),
    CONSTRAINT `Notifications_Event` FOREIGN KEY (eventID) REFERENCES Events (ID),
    CONSTRAINT `Notifications_Participant` FOREIGN KEY (participantID) REFERENCES EventParticipants (ID)
);