CREATE TABLE gift_registry_db.groupsGifts (
    GroupGiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    GiftID INT NOT NULL
);

ALTER TABLE gift_registry_db.groupsGifts
    ADD CONSTRAINT FK_GroupsGifts_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);

ALTER TABLE gift_registry_db.groupsGifts
    ADD CONSTRAINT FK_GroupsGifts_Gifts FOREIGN KEY (GiftID)
        REFERENCES gift_registry_db.gifts(GiftID);