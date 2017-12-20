CREATE TABLE gift_registry_db.users (
    UserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL,
    PasswordID INT NOT NULL,
    UserEmail VARCHAR(255) NOT NULL UNIQUE,
    UserTheme TINYINT UNSIGNED NULL DEFAULT 0,
    UserBio VARCHAR(4095) NULL,
    UserBirthMonth INT NULL DEFAULT 0,
    UserBirthDay INT NULL DEFAULT 0,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE gift_registry_db.users
    ADD CONSTRAINT FK_UsersPassword FOREIGN KEY (PasswordID)
        REFERENCES gift_registry_db.passwords(PasswordID);