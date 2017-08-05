CREATE TABLE gift_registry_db.reservations (
    ReservationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GiftID INT NOT NULL,
    UserID INT NOT NULL,
    DateReceived DATE NULL
);

ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsGifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);

ALTER TABLE gift_registry_db.reservations
    ADD CONSTRAINT FK_ReservationsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);