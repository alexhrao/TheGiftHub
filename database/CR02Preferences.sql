CREATE TABLE gift_registry_db.preferences (
    PreferenceID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    PreferenceName VARCHAR(255) NOT NULL,
    ValueTypeID INT NOT NULL
);
ALTER TABLE gift_registry_db.preferences
    ADD CONSTRAINT FK_PreferencesValues FOREIGN KEY (ValueTypeID)
        REFERENCES gift_registry_db.value_types(ValueTypeID);