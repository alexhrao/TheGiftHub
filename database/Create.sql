CREATE DATABASE gift_registry_db;

CREATE TABLE gift_registry_db.categories (
    CategoryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(255) NOT NULL,
    CategoryDescription VARCHAR(4096) NULL
);

CREATE TABLE gift_registry_db.passwords (
    PasswordID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    PasswordHash CHAR(66) NOT NULL,
    CreateStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE gift_registry_db.defaultEvents (
    GiftEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftEventDay INT NOT NULL,
    GiftEventMonth INT NOT NULL,
    GiftEventName VARCHAR(255) NOT NULL,
    GiftEventDescription VARCHAR(4096) NULL
);

CREATE TABLE gift_registry_db.users (
    UserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PasswordID INT NOT NULL,
    UserEmail VARCHAR(255) NOT NULL UNIQUE,
    UserTheme TINYINT UNSIGNED DEFAULT 0,
    UserImage VARCHAR(255) NOT NULL DEFAULT 'default.png',
    DateOfBirth DATE NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.users
    ADD CONSTRAINT FK_UsersPassword FOREIGN KEY (PasswordID)
        REFERENCES gift_registry_db.passwords(PasswordID);

CREATE TABLE gift_registry_db.giftEventsUsers (
    GiftEventUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    EventDay INT NOT NULL,
    EventMonth INT NOT NULL,
    EventName VARCHAR(255) NOT NULL,
    EventDescription VARCHAR(4096) NULL
);
ALTER TABLE gift_registry_db.giftEventUsers
    ADD CONSTRAINT FK_giftEvent_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users;
        
CREATE TABLE gift_registry_db.groups (
    GroupID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupName VARCHAR(255) NOT NULL,
    GroupDescription VARCHAR(4095) NULL,
    AdminID INT NOT NULL
);
ALTER TABLE gift_registry_db.groups
    ADD CONSTRAINT FK_GroupsUser FOREIGN KEY (AdminID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.passwordResets (
    PasswordResetID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ResetHash CHAR(88) NOT NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.passwordResets
    ADD CONSTRAINT FK_PasswordResetsUser FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.gifts (
    GiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    GiftName VARCHAR(255) NOT NULL,
    GiftDescription VARCHAR(8191) NULL,
    GiftURL VARCHAR(1023) NULL,
    GiftCost DECIMAL(15, 2) NULL,
    GiftStores VARCHAR(4095) NULL,
    GiftQuantity INT UNSIGNED NOT NULL DEFAULT 0,
    GiftPicture VARCHAR(255) NULL,
    GiftSize VARCHAR(127) NULL,
    CategoryID INT NOT NULL,
    GiftRating DECIMAL(3, 2) NOT NULL DEFAULT 0,
    GiftAddDate DATE NULL,
    GiftReceivedDate DATE NULL
);
ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsCategories FOREIGN KEY (CategoryID)
        REFERENCES gift_registry_db.categories(CategoryID);
        
CREATE TABLE gift_registry_db.groupsUsers (
    GroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    UserID INT NOT NULL
);
ALTER TABLE gift_registry_db.groupsUsers
    ADD CONSTRAINT FK_GroupsUsers_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.groupsUsers
    ADD CONSTRAINT FK_GroupsUsers_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);

CREATE TABLE gift_registry_db.receptions (
    ReceptionID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    NumReceived INT NOT NULL DEFAULT 0,
    DateReceived DATE NULL
);
ALTER TABLE gift_registry_db.receptions
    ADD CONSTRAINT FK_GiftsReceptions FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);

CREATE TABLE gift_registry_db.reservations (
    ReservationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    UserID INT NOT NULL,
    DateReceived DATE NULL
);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsGifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.groupsGifts (
    GroupGiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    GiftID INT NOT NULL
);
ALTER TABLE gift_registry_db.groupsGifts
    ADD CONSTRAINT FK_GroupsGifts_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);
ALTER TABLE gift_registry_db.groupsGifts
    ADD CONSTRAINT FK_GroupsGifts_Gifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);