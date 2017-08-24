CREATE DATABASE gift_registry_db;

CREATE TABLE gift_registry_db.categories (
    CategoryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(255) NOT NULL,
    CategoryDescription VARCHAR(4096) NULL
);

CREATE TABLE gift_registry_db.value_types (
    ValueTypeID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ValueTypeName VARCHAR(255) NOT NULL
);

CREATE TABLE gift_registry_db.default_events (
    EventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventDay INT NOT NULL,
    EventMonth INT NOT NULL,
    EventYear INT NOT NULL,
    EventRecurs BOOLEAN NOT NULL DEFAULT TRUE,
    EventName VARCHAR(255) NOT NULL,
    EventDescription VARCHAR(4095) NULL
);

CREATE TABLE gift_registry_db.preferences (
    PreferenceID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    PreferenceName VARCHAR(255) NOT NULL,
    ValueTypeID INT NOT NULL
);
ALTER TABLE gift_registry_db.preferences
    ADD CONSTRAINT FK_PreferencesValues FOREIGN KEY (ValueTypeID)
        REFERENCES gift_registry_db.value_types(ValueTypeID);

CREATE TABLE gift_registry_db.default_events_futures (
    EventFutureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL,
    EventYear INT NOT NULL,
    EventMonth INT NOT NULL,
    EventDay INT NOT NULL
);
ALTER TABLE gift_registry_db.default_events_futures
    ADD CONSTRAINT FK_DefaultEventsFuture FOREIGN KEY (EventID)
        REFERENCES gift_registry_db.default_events(EventID);

CREATE TABLE gift_registry_db.users (
    UserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    UserEmail VARCHAR(255) NOT NULL UNIQUE,
    UserTheme TINYINT UNSIGNED NULL DEFAULT 0,
    UserBio VARCHAR(4095) NULL,
    UserBirthMonth INT NOT NULL DEFAULT 0,
    UserBirthDay INT NOT NULL DEFAULT 0,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

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

CREATE TABLE gift_registry_db.events_users (
    EventUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    EventID INT NULL,
    EventDay INT NOT NULL,
    EventMonth INT NOT NULL,
    EventYear INT NOT NULL,
    EventRecurs BOOLEAN NOT NULL DEFAULT TRUE,
    EventName VARCHAR(255) NOT NULL,
    EventDescription VARCHAR(4095) NULL
);
ALTER TABLE gift_registry_db.events_users
    ADD CONSTRAINT FK_Event_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.events_users
    ADD CONSTRAINT FK_Event_Events FOREIGN KEY (EventID)
        REFERENCES gift_registry_db.default_events(EventID);
        
CREATE TABLE gift_registry_db.users_preferences (
    UserPreferenceID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    PreferenceID INT NOT NULL,
    PreferenceValue VARCHAR(4095) NOT NULL
);
ALTER TABLE gift_registry_db.users_preferences
    ADD CONSTRAINT FK_PreferencesUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);
ALTER TABLE gift_registry_db.users_preferences
    ADD CONSTRAINT FK_PreferencesPrefs FOREIGN KEY (PreferenceID)
        REFERENCES gift_registry_db.preferences(PreferenceID);
        
CREATE TABLE gift_registry_db.events_users_futures (
    EventFutureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventUserID INT NOT NULL,
    EventYear INT NOT NULL,
    EventMonth INT NOT NULL,
    EventDay INT NOT NULL
);
ALTER TABLE gift_registry_db.events_users_futures
    ADD CONSTRAINT FK_EventsFuture FOREIGN KEY (EventUserID)
        REFERENCES gift_registry_db.events_users(EventUserID);
    
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
    ReserveStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsGifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);
ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

CREATE TABLE gift_registry_db.events_users_groups (
    EventGroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventUserID INT NOT NULL,
    GroupID INT NOT NULL
);
ALTER TABLE gift_registry_db.events_users_groups
    ADD CONSTRAINT FK_EventsUsersGroups_Events FOREIGN KEY (EventUserID)
        REFERENCES gift_registry_db.events_users(EventUserID);
ALTER TABLE gift_registry_db.events_users_groups
    ADD CONSTRAINT FK_EventsUsersGroups_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);
        
CREATE TABLE gift_registry_db.purchases (
    PurchaseID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ReservationID INT NOT NULL,
    PurchaseStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.purchases
    ADD CONSTRAINT FK_PurchasesReservations FOREIGN KEY (ReservationID)
        REFERENCES gift_registry_db.reservations(ReservationID);