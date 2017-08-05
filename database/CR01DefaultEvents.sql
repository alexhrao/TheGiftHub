CREATE TABLE gift_registry_db.defaultEvents (
    GiftEventID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftEventDay INT NOT NULL,
    GiftEventMonth INT NOT NULL,
    GiftEventName VARCHAR(255) NOT NULL,
    GiftEventDescription VARCHAR(4095) NULL
);