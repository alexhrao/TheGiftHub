CREATE DATABASE gift_registry_db;

CREATE TABLE gift_registry_db.cultures (
    CultureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CultureLanguage CHAR(2) NOT NULL,
    CultureLocation CHAR(2) NOT NULL,
    CultureName VARCHAR(63) NULL,
    CultureDesc VARCHAR(255) NULL
);

CREATE TABLE gift_registry_db.categories (
    CategoryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(255) NOT NULL
);

CREATE TABLE gift_registry_db.users (
    UserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    UserEmail VARCHAR(255) NOT NULL UNIQUE,
    UserBio VARCHAR(4095) NULL,
    UserBirthMonth INT NOT NULL DEFAULT 0,
    UserBirthDay INT NOT NULL DEFAULT 0,
    UserURL CHAR(28) NOT NULL UNIQUE,
    UserGoogleID VARCHAR(1024) NULL UNIQUE,
    UserFacebookID VARCHAR(1024) NULL UNIQUE,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
        
/* This is a one-to-one relationship - just for simplicity, it has been moved to a different table */
CREATE TABLE gift_registry_db.preferences (
    PreferenceID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    UserCulture CHAR(5) NOT NULL,
    UserTheme TINYINT UNSIGNED NULL DEFAULT 0
);
ALTER TABLE gift_registry_db.preferences
    ADD CONSTRAINT FK_UsersPreferences FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
        
CREATE TABLE gift_registry_db.passwords (
    PasswordID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    PasswordHash CHAR(28) NOT NULL,
    PasswordSalt CHAR(24) NOT NULL,
    PasswordIter INT UNSIGNED NOT NULL,
    CreateStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.passwords
    ADD CONSTRAINT FK_PasswordsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
    
CREATE TABLE gift_registry_db.groups (
    GroupID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupName VARCHAR(255) NOT NULL,
    GroupDescription VARCHAR(4095) NULL,
    AdminID INT NOT NULL
);
ALTER TABLE gift_registry_db.groups
    ADD CONSTRAINT FK_GroupsUser FOREIGN KEY (AdminID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.passwordresets (
    PasswordResetID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ResetHash CHAR(88) NOT NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.passwordresets
    ADD CONSTRAINT FK_PasswordResetsUser FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.user_events (
	EventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    EventStartDate DATE NOT NULL,
    EventEndDate DATE NULL,
    EventName VARCHAR(256) NOT NULL
);
ALTER TABLE gift_registry_db.user_events
	ADD CONSTRAINT FK_EventsUsers FOREIGN KEY (UserID)
		REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.exact_events (
	ExactEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    EventTimeInterval CHAR(1) NOT NULL,
    EventSkipEvery INT NOT NULL # ONLY POSITIVE
);
ALTER TABLE gift_registry_db.exact_events
	ADD CONSTRAINT FK_ExactEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db.user_events(EventID);
        
CREATE TABLE gift_registry_db.relative_events (
	RelativeEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    EventTimeInterval CHAR(3) NULL,
    EventSkipEvery INT NOT NULL, # ONLY POSITIVE
    EventDayOfWeek CHAR(1) NOT NULL,
    EventPosn INT NOT NULL # ONLY 1-5
);
ALTER TABLE gift_registry_db.relative_events
	ADD CONSTRAINT FK_RelativeEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db.user_events(EventID);

CREATE TABLE gift_registry_db.event_blackouts (
	EventBlackoutID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL,
    EventBlackoutDate DATE NOT NULL
);
ALTER TABLE gift_registry_db.event_blackouts
	ADD CONSTRAINT FK_BlackoutEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db.user_events(EventID);
        
CREATE TABLE gift_registry_db.gifts (
    GiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    GiftName VARCHAR(255) NOT NULL,
    GiftDescription VARCHAR(8191) NULL,
    GiftURL VARCHAR(1023) NULL,
    GiftCost DECIMAL(15, 2) NULL,
    GiftStores VARCHAR(4095) NULL,
    GiftQuantity INT UNSIGNED NOT NULL DEFAULT 0,
    GiftColor VARCHAR(6) NULL,
    GiftColorText VARCHAR(31) NULL,
    GiftSize VARCHAR(127) NULL,
    CategoryID INT NOT NULL,
    GiftRating DECIMAL(3, 2) NOT NULL DEFAULT 0,
    GiftAddStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    GiftReceivedDate DATE NULL
);
ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsCategories FOREIGN KEY (CategoryID)
        REFERENCES gift_registry_db.categories(CategoryID);
        
CREATE TABLE gift_registry_db.groups_users (
    GroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    UserID INT NOT NULL,
    IsChild BOOLEAN NOT NULL DEFAULT FALSE
);
ALTER TABLE gift_registry_db.groups_users
    ADD CONSTRAINT FK_GroupsUsers_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.groups_users
    ADD CONSTRAINT FK_GroupsUsers_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);

CREATE TABLE gift_registry_db.groups_gifts (
    GroupGiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    GiftID INT NOT NULL
);
ALTER TABLE gift_registry_db.groups_gifts
    ADD CONSTRAINT FK_GroupsGifts_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);
ALTER TABLE gift_registry_db.groups_gifts
    ADD CONSTRAINT FK_GroupsGifts_Gifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);

CREATE TABLE gift_registry_db.groups_events (
	GroupEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    EventID INT NOT NULL
);
ALTER TABLE gift_registry_db.groups_events
    ADD CONSTRAINT FK_GroupsEvents_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);
ALTER TABLE gift_registry_db.groups_events
    ADD CONSTRAINT FK_GroupsEvents_Events FOREIGN KEY (EventID)
        REFERENCES gift_registry_db.user_events(EventID);

CREATE TABLE gift_registry_db.receptions (
    ReceptionID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    NumReceived INT NOT NULL DEFAULT 0,
    ReceptionStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.receptions
    ADD CONSTRAINT FK_GiftsReceptions FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);

CREATE TABLE gift_registry_db.reservations (
    ReservationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    UserID INT NOT NULL,
    PurchaseStamp TIMESTAMP NULL,
    ReserveStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsGifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);