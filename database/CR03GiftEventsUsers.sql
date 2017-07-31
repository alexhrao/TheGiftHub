CREATE TABLE gift_registry_db.giftEventsUsers (
    GiftEventUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftEventID INT NOT NULL,
    UserID INT NOT NULL,
    EventDay INT NOT NULL,
    EventMonth INT NOT NULL,
    EventName VARCHAR(255) NOT NULL,
    EventDescription VARCHAR(4096) NULL
);

ALTER TABLE gift_registry_db.giftEventUsers
    ADD CONSTRAINT FK_giftEvent_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users;

ALTER TABLE gift_registry_db.giftEventUsers
    ADD CONSTRAINT FK_giftEvent_Events FOREIGN KEY (GiftEventID)
        REFERENCES gift_registry_db.giftEvents;