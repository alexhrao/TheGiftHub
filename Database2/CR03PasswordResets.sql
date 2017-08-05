CREATE TABLE gift_registry_db.passwordResets (
    PasswordResetID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ResetHash CHAR(88) NOT NULL,
    TimeCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE gift_registry_db.passwordResets
    ADD CONSTRAINT FK_PasswordResetsUser FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);