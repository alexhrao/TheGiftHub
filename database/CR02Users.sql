CREATE TABLE gift_registry_db.users (
    UserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PasswordID INT NOT NULL,
    UserEmail VARCHAR(255) NOT NULL UNIQUE,
    UserTheme TINYINT UNSIGNED DEFAULT 0,
    UserImage VARCHAR(255) NOT NULL DEFAULT "default.png",
    DateOfBirth DATE NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE gift_registry_db.users
    ADD CONSTRAINT FK_UsersPassword FOREIGN KEY (PasswordID)
        REFERENCES gift_registry_db.passwords(PasswordID);