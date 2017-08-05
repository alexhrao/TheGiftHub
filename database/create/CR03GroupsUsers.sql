CREATE TABLE gift_registry_db.groupsUsers (
    GroupUserID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    GroupID INT NOT NULL,
    UserID INT NOT NULL,
    IsChild BOOL NOT NULL DEFAULT FALSE
);

ALTER TABLE gift_registry_db.groupsUsers
    ADD CONSTRAINT FK_GroupsUsers_Users FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

ALTER TABLE gift_registry_db.groupsUsers
    ADD CONSTRAINT FK_GroupsUsers_Groups FOREIGN KEY (GroupID)
        REFERENCES gift_registry_db.groups(GroupID);