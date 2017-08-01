CREATE TABLE gift_registry_db.eventsUsersGroups (
    EventGroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    EventUserID INT NOT NULL,
    GroupID INT NOT NULL
);

ALTER TABLE gift_registry_db.eventsUsersGroups
    ADD CONSTRAINT FK_EventsUsersGroups_Events FOREIGN KEY (EventUserID)
        REFERENCES gift_registry_db.eventsusers(EventUserID);

ALTER TABLE gift_registry_db.eventsUsersGroups
    ADD CONSTRAINT FK_EventsUsersGroups_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);