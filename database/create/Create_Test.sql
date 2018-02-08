CREATE DATABASE gift_registry_db_test;

CREATE TABLE gift_registry_db_test.cultures (
    CultureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CultureLanguage CHAR(2) NOT NULL,
    CultureLocation CHAR(2) NOT NULL,
    CultureName VARCHAR(63) NULL,
    CultureDesc VARCHAR(255) NULL
);

CREATE TABLE gift_registry_db_test.categories (
    CategoryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(255) NOT NULL,
    CategoryDescription VARCHAR(4096) NULL
);

CREATE TABLE gift_registry_db_test.users (
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
CREATE TABLE gift_registry_db_test.preferences (
    PreferenceID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    UserCulture CHAR(5) NOT NULL,
    UserTheme TINYINT UNSIGNED NULL DEFAULT 0
);
ALTER TABLE gift_registry_db_test.preferences
    ADD CONSTRAINT FK_UsersPreferences FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);
        
CREATE TABLE gift_registry_db_test.passwords (
    PasswordID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    PasswordHash CHAR(28) NOT NULL,
    PasswordSalt CHAR(24) NOT NULL,
    PasswordIter INT UNSIGNED NOT NULL,
    CreateStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db_test.passwords
    ADD CONSTRAINT FK_PasswordsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);
    
CREATE TABLE gift_registry_db_test.groups (
    GroupID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupName VARCHAR(255) NOT NULL,
    GroupDescription VARCHAR(4095) NULL,
    AdminID INT NOT NULL
);
ALTER TABLE gift_registry_db_test.groups
    ADD CONSTRAINT FK_GroupsUser FOREIGN KEY (AdminID)
        REFERENCES gift_registry_db_test.users(UserID);

CREATE TABLE gift_registry_db_test.passwordresets (
    PasswordResetID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ResetHash CHAR(88) NOT NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db_test.passwordresets
    ADD CONSTRAINT FK_PasswordResetsUser FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);

CREATE TABLE gift_registry_db_test.user_events (
	EventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    EventStartDate DATE NOT NULL,
    EventEndDate DATE NULL,
    EventName VARCHAR(256) NOT NULL
);
ALTER TABLE gift_registry_db_test.user_events
	ADD CONSTRAINT FK_EventsUsers FOREIGN KEY (UserID)
		REFERENCES gift_registry_db_test.users(UserID);

CREATE TABLE gift_registry_db_test.exact_events (
	ExactEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    EventTimeInterval CHAR(1) NOT NULL,
    EventSkipEvery INT NOT NULL # ONLY POSITIVE
);
ALTER TABLE gift_registry_db_test.exact_events
	ADD CONSTRAINT FK_ExactEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db_test.user_events(EventID);
        
CREATE TABLE gift_registry_db_test.relative_events (
	RelativeEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    EventTimeInterval CHAR(3) NULL,
    EventSkipEvery INT NOT NULL, # ONLY POSITIVE
    EventDayOfWeek CHAR(1) NOT NULL,
    EventPosn INT NOT NULL # ONLY 1-5
);
ALTER TABLE gift_registry_db_test.relative_events
	ADD CONSTRAINT FK_RelativeEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db_test.user_events(EventID);

CREATE TABLE gift_registry_db_test.event_blackouts (
	EventBlackoutID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL,
    EventBlackoutDate DATE NOT NULL
);
ALTER TABLE gift_registry_db_test.event_blackouts
	ADD CONSTRAINT FK_BlackoutEvents FOREIGN KEY (EventID)
		REFERENCES gift_registry_db_test.user_events(EventID);
        
CREATE TABLE gift_registry_db_test.gifts (
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
ALTER TABLE gift_registry_db_test.gifts
    ADD CONSTRAINT FK_GiftsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);
ALTER TABLE gift_registry_db_test.gifts
    ADD CONSTRAINT FK_GiftsCategories FOREIGN KEY (CategoryID)
        REFERENCES gift_registry_db_test.categories(CategoryID);
        
CREATE TABLE gift_registry_db_test.groups_users (
    GroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    UserID INT NOT NULL,
    IsChild BOOLEAN NOT NULL DEFAULT FALSE
);
ALTER TABLE gift_registry_db_test.groups_users
    ADD CONSTRAINT FK_GroupsUsers_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);
ALTER TABLE gift_registry_db_test.groups_users
    ADD CONSTRAINT FK_GroupsUsers_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db_test.groups(GroupID);

CREATE TABLE gift_registry_db_test.groups_gifts (
    GroupGiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    GiftID INT NOT NULL
);
ALTER TABLE gift_registry_db_test.groups_gifts
    ADD CONSTRAINT FK_GroupsGifts_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db_test.groups(GroupID);
ALTER TABLE gift_registry_db_test.groups_gifts
    ADD CONSTRAINT FK_GroupsGifts_Gifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db_test.gifts(GiftID);

CREATE TABLE gift_registry_db_test.groups_events (
	GroupEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    EventID INT NOT NULL
);
ALTER TABLE gift_registry_db_test.groups_events
    ADD CONSTRAINT FK_GroupsEvents_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db_test.groups(GroupID);
ALTER TABLE gift_registry_db_test.groups_events
    ADD CONSTRAINT FK_GroupsEvents_Events FOREIGN KEY (EventID)
        REFERENCES gift_registry_db_test.user_events(EventID);

CREATE TABLE gift_registry_db_test.receptions (
    ReceptionID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    NumReceived INT NOT NULL DEFAULT 0,
    ReceptionStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db_test.receptions
    ADD CONSTRAINT FK_GiftsReceptions FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db_test.gifts(GiftID);

CREATE TABLE gift_registry_db_test.reservations (
    ReservationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    UserID INT NOT NULL,
    PurchaseStamp TIMESTAMP NULL,
    ReserveStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db_test.reservations
    ADD CONSTRAINT FK_ReservationsGifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db_test.gifts(GiftID);
ALTER TABLE gift_registry_db_test.reservations
    ADD CONSTRAINT FK_ReservationsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db_test.users(UserID);



#### LOAD Procedure ####

DELIMITER $$
USE `gift_registry_db_test`$$
CREATE PROCEDURE `setup` ()
BEGIN
	-- Delete old values
	DELETE FROM gift_registry_db_test.reservations;
	DELETE FROM gift_registry_db_test.groups_gifts;
	DELETE FROM gift_registry_db_test.groups_events;
	DELETE FROM gift_registry_db_test.groups_users;
	DELETE FROM gift_registry_db_test.event_blackouts;
	DELETE FROM gift_registry_db_test.relative_events;
	DELETE FROM gift_registry_db_test.exact_events;
	DELETE FROM gift_registry_db_test.user_events;
	DELETE FROM gift_registry_db_test.groups;
	DELETE FROM gift_registry_db_test.gifts;
	DELETE FROM gift_registry_db_test.passwordresets;
	DELETE FROM gift_registry_db_test.passwords;
	DELETE FROM gift_registry_db_test.cultures;
	DELETE FROM gift_registry_db_test.categories;
	DELETE FROM gift_registry_db_test.preferences;
	DELETE FROM gift_registry_db_test.users;
	-- Create categories:
	INSERT INTO gift_registry_db_test.categories (CategoryID, CategoryName, CategoryDescription)
		VALUES (1, 'Clothing', 'Clothing');
	INSERT INTO gift_registry_db_test.categories (CategoryID, CategoryName, CategoryDescription)
		VALUES (2, 'Electronics', 'Electronic Devices');

	-- Create users:
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserFacebookID, UserGoogleID)
		VALUES (1, 'Alex Rao', 'alexhrao@gmail.com', 7, 3, "I LOVE to have fun!", "UJIOl8xUU7ccIQH5Ofs0Awfdsa00", "12345", "12345");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
		VALUES (2, 'Raeedah Choudhury', 'raeedah.choudhury@gmail.com', 11, 19, "I love my boyfriend", "UJIOl8xUU7ccIQH5Ofs0Awfdsa11");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
		VALUES (3, 'Alex Tes', 'alexhrao@gatech.edu', 7, 3, "fdsafdsa", "UJIOl7xUU7ccIdH5Ofs0Awfdsa00");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
		VALUES (4, 'Alex Tes', 'arao81@gatech.edu', 7, 3, "fdsafdsa", "UqIOl7xfU7ccIQH5Ofs0Awfdsa00");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserFacebookID, UserGoogleID)
		VALUES (5, 'ToDelete', 'alexhrao@hotmail.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd8765", "54321", "54321");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserFacebookID, UserGoogleID)
		VALUES (6, 'NoImage', 'alexhrao@github.edu', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd1234", "43215", "43215");
	INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserFacebookID, UserGoogleID)
		VALUES (7, 'HasImage', 'alexhrao@google.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd2222", "12312", "12312");
	-- Create preferences:
	INSERT INTO gift_registry_db_test.preferences (UserID, UserCulture, UserTheme)
		VALUES (1, 'en-US', 0);
	INSERT INTO gift_registry_db_test.preferences (UserID, UserCulture, UserTheme)
		VALUES (2, 'fr-FR', 1);
		
	-- Create Password:
	INSERT INTO gift_registry_db_test.passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
		VALUES (1, 'xITVQ4AJ9eLfuzeG+cxeajH3QRw=', 'lN6ivSpluwewZWZCVXFNBw==', 10000);

	INSERT INTO gift_registry_db_test.passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
		VALUES (2, '12345678901234567890', '1234567890123567', 10000);

	-- Create Events
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (1, 1, '2017-01-10', NULL, 'Exact Event Test');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (2, 1, '2017-01-7', '2020-01-01', 'Relative Event Test');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (3, 1, '2017-08-04', '2017-08-04', 'One Off Event Test');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (4, 1, '2017-01-05', NULL, 'Blackout Date Test');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (5, 2, '2017-01-10', NULL, 'Exact Event Test 2');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (6, 2, '2017-01-7', '2020-01-01', 'Relative Event Test 2');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (7, 2, '2017-08-04', '2017-08-04', 'One Off Event Test 2');
	INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
		VALUES (8, 2, '2017-04-04', NULL, 'Blackout Date Test 2');

	-- Create groups
	INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
		VALUES (1, 'The Rao''s', 1);
	INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
		VALUES (2, 'Test Group 2', 2);
	INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
		VALUES (3, 'Test Group 3', 4);
	INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
		VALUES (4, 'Test Group 1 is out', 4);
		
	-- Create gift
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (1, 1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'FFFFFF', 'White', 'N/A', 2, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (2, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1000, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (3, 1, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 3, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (4, 7, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 5, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (5, 2, 'Wafdsav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (6, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (7, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 5600, '000000', 'black', 'BIG', 1, 5.0);
	INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
		VALUES (8, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0);

	-- Add user to group
	INSERT INTO gift_registry_db_test.groups_users (GroupID, UserID, IsChild)
		VALUES (1, 2, TRUE);
	INSERT INTO gift_registry_db_test.groups_users (GroupID, UserID)
		VALUES 
			(2, 3),
			(1, 1),
			(2, 1),
			(2, 5),
			(3, 1),
			(3, 2),
			(4, 2),
			(4, 3),
            (4, 6);

	-- Add gift to group
	INSERT INTO gift_registry_db_test.groups_gifts (GroupID, GiftID)
		VALUES (1, 1);
	INSERT INTO gift_registry_db_test.groups_gifts (GroupID, GiftID)
		VALUES (1, 2);

	-- Create the Exact Event Rule
	INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
		VALUES (1, 'M', 1);
	INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
		VALUES (5, 'W', 3);
	INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
		VALUES (4, 'M', 1);
		
	-- Create the Relative Event Rule
	INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
		VALUES (2, 'MAR', 1, 'M', 2);
	INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
		VALUES (6, NULL, 2, 'S', 3);
	INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
		VALUES (8, NULL, 1, 'T', 5);

	-- Create Blackout Dates
	INSERT INTO gift_registry_db_test.event_blackouts (EventID, EventBlackoutDate)
		VALUES (4, '2017-02-05');
	INSERT INTO gift_registry_db_test.event_blackouts (EventID, EventBlackoutDate)
		VALUES (8, '2017-06-27');

	-- Add events to groups
	INSERT INTO gift_registry_db_test.groups_events (GroupID, EventID)
		VALUES
			(1, 1),
			(1, 2),
			(1, 3),
			(2, 4),
			(1, 5),
			(2, 6),
			(2, 7),
			(4, 8);
		
	-- Add Reservations
    INSERT INTO gift_registry_db_test.reservations (ReservationID, GiftID, UserID, PurchaseStamp)
		VALUES
			( 1, 6, 1, NULL),
            ( 2, 3, 1, '2017-03-09'),
            ( 3, 3, 1, NULL),
            ( 4, 3, 1, NULL),
            ( 5, 8, 2, '2017-08-09'),
            ( 6, 7, 1, NULL),
            ( 7, 5, 4, NULL),
            ( 8, 5, 4, NULL),
            ( 9, 5, 4, NULL),
            (10, 5, 4, NULL),
            (11, 5, 4, NULL),
            (12, 7, 6, NULL),
            (13, 2, 3, NULL),
            (14, 2, 3, NULL),
            (15, 2, 3, NULL),
            (16, 2, 7, NULL),
            (17, 7, 2, '2017-08-09'),
            (18, 7, 2, '2017-08-10'),
            (19, 7, 2, '2017-08-09'),
            (20, 7, 2, '2017-08-14'),
            (21, 2, 1, '2017-02-02'),
            (22, 2, 1, '2017-06-02'),
            (23, 2, 1, '2017-04-02');
            
	-- Add Cultures
	INSERT INTO gift_registry_db_test.cultures (CultureID, CultureLanguage, CultureLocation, CultureName, CultureDesc)
		VALUES
		(1, 'en', 'US', 'English (US)', 'English (United States)'),
		(2, 'fr', 'FR', 'French (FR)', 'French'),
		(3, 'en', 'GB', 'English (UK)', 'English (United Kingdom)');
END$$
DELIMITER ;

CALL gift_registry_db_test.setup();