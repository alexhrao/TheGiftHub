USE gift_registry_db;
-- Insert into categories:
INSERT INTO categories (categories.CategoryName, categories.CategoryDescription) VALUES ('Clothing', 'Clothing');
INSERT INTO categories (categories.CategoryName, categories.CategoryDescription) VALUES ('Electronics', 'Electronic Devices');
-- Default Events:
INSERT INTO defaultevents (defaultevents.EventName, defaultevents.EventMonth, defaultevents.EventDay) VALUES ('Christmas', 12, 25);
INSERT INTO defaultevents (defaultevents.EventName, defaultevents.EventMonth, defaultevents.EventDay) VALUES ('New Year''s', 1, 1);
-- Insert into Password:
INSERT INTO passwords (passwords.PasswordHash) VALUES ('fdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafd');
INSERT INTO passwords (passwords.PasswordHash) VALUES ('fdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsa32');
-- Insert into users:
INSERT INTO users (users.FirstName, users.LastName, users.UserEmail, users.DateOfBirth, users.PasswordID) VALUES ('Alex', 'Rao', 'alexhrao@gmail.com', "1996-07-03", 1);
INSERT INTO users (users.FirstName, users.LastName, users.UserEmail, users.DateOfBirth, users.PasswordID) VALUES ('Raeedah', 'Choudhury', 'rchoudhury@gmail.com', "1997-11-19", 2);
-- Insert into events:
INSERT INTO eventsusers (eventsusers.EventName, eventsusers.UserID, eventsusers.EventMonth, eventsusers.EventDay, eventsusers.EventID) VALUES ('Christmas', 1, 12, 25, 1);
INSERT INTO eventsusers (eventsusers.EventName, eventsusers.UserID, eventsusers.EventMonth, eventsusers.EventDay, eventsusers.EventID) VALUES ('New Year''s', 2, 1, 1, 2);
INSERT INTO eventsusers (eventsusers.EventName, eventsusers.UserID, eventsusers.EventMonth, eventsusers.EventDay, eventsusers.EventID) VALUES ('Christmas', 2, 12, 25, 1);
-- Create group
INSERT INTO groups (groups.GroupName, groups.AdminID) VALUES ('The Rao''s', 1);
-- Create gift
INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftPicture, GiftSize, CategoryID, GiftRating)
    VALUES (1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'default.png', 'N/A', 2, 5.0);
-- Add user to group
INSERT INTO groupsusers (groupsusers.GroupID, groupsusers.UserID) VALUES (1, 1);
INSERT INTO groupsusers (groupsusers.GroupID, groupsusers.UserID) VALUES (1, 2);
-- Add gift to group
INSERT INTO groupsgifts (GroupID, GiftID) VALUES (1, 1);
-- Make new year's visible to group
INSERT INTO eventsusersgroups (EventUserID, GroupID) VALUES (2, 1);
INSERT INTO eventsusersgroups (EventUserID, GroupID) VALUES (3, 1);