CREATE TABLE gift_registry_db.default_events_futures (
    EventFutureID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventID INT NOT NULL,
    EventYear INT NOT NULL,
    EventMonth INT NOT NULL,
    EventDay INT NOT NULL
);
ALTER TABLE gift_registry_db.default_events_futures
    ADD CONSTRAINT FK_DefaultEventsFuture FOREIGN KEY (EventID)
        REFERENCES gift_registry_db.default_events(EventID);