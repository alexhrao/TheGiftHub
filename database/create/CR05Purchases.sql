CREATE TABLE gift_registry_db.purchases (
    PurchaseID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ReservationID INT NOT NULL,
    PurchaseStamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE gift_registry_db.purchases
    ADD CONSTRAINT FK_PurchasesReservations FOREIGN KEY (ReservationID)
        REFERENCES gift_registry_db.reservations(ReservationID);