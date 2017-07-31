CREATE TABLE gift_registry_db.giftEvents (
    GiftEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftEventDay INT NOT NULL,
    GiftEventMonth INT NOT NULL,
    GiftEventName VARCHAR(255) NOT NULL,
    GiftEventDescription VARCHAR(4096) NULL
);