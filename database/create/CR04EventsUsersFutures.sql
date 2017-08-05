CREATE TABLE gift_registry_db.events_users_futures (
    EventFutureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventUserID INT NOT NULL,
    EventYear INT NOT NULL,
    EventMonth INT NOT NULL,
    EventDay INT NOT NULL
);
ALTER TABLE gift_registry_db.events_users_futures
    ADD CONSTRAINT FK_DefaultEventsFuture FOREIGN KEY (EventUserID)
        REFERENCES gift_registry_db.events_users(EventUserID);