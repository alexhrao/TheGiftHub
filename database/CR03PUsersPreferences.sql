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