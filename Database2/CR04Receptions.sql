CREATE TABLE gift_registry_db.receptions (
    ReceptionID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    NumReceived INT NOT NULL DEFAULT 0,
    DateReceived DATE NULL
);

ALTER TABLE gift_registry_db.receptions
    ADD CONSTRAINT FK_GiftsReceptions FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);