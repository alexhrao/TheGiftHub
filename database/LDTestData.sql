USE gift_registry_db;
-- Insert into categories:
INSERT INTO categories (categories.CategoryName, categories.CategoryDescription) VALUES ('Clothing', 'Clothing');
INSERT INTO categories (categories.CategoryName, categories.CategoryDescription) VALUES ('Electronics', 'Electronic Devices');
-- Default Events:
INSERT INTO default_events (default_events.EventName, default_events.EventYear, default_events.EventMonth, default_events.EventDay, default_events.EventRecurs) VALUES ('Christmas', 2017, 12, 25, TRUE);
INSERT INTO default_events (default_events.EventName, default_events.EventYear, default_events.EventMonth, default_events.EventDay, default_events.EventRecurs) VALUES ('New Year''s', 2017, 1, 1, TRUE);
-- Insert into Password:
INSERT INTO passwords (passwords.PasswordHash) VALUES ('fdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafd');
INSERT INTO passwords (passwords.PasswordHash) VALUES ('fdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsafdfdsa32');
-- Insert into users:
INSERT INTO users (users.FirstName, users.LastName, users.UserEmail, users.DateOfBirth, users.PasswordID) VALUES ('Alex', 'Rao', 'alexhrao@gmail.com', "1996-07-03", 1);
INSERT INTO users (users.FirstName, users.LastName, users.UserEmail, users.DateOfBirth, users.PasswordID) VALUES ('Raeedah', 'Choudhury', 'rchoudhury@gmail.com', "1997-11-19", 2);
-- Insert into events:
INSERT INTO events_users (events_users.EventName, events_users.UserID, events_users.EventYear, events_users.EventMonth, events_users.EventDay, events_users.EventID, events_users.EventRecurs) VALUES ('Christmas', 1, 2017, 12, 25, 1, TRUE);
INSERT INTO events_users (events_users.EventName, events_users.UserID, events_users.EventYear, events_users.EventMonth, events_users.EventDay, events_users.EventID, events_users.EventRecurs) VALUES ('New Year''s', 2, 2017, 1, 1, 2, TRUE);
INSERT INTO events_users (events_users.EventName, events_users.UserID, events_users.EventYear, events_users.EventMonth, events_users.EventDay, events_users.EventID, events_users.EventRecurs) VALUES ('Christmas', 2, 2017, 12, 25, 1, TRUE);
-- Create group
INSERT INTO groups (groups.GroupName, groups.AdminID) VALUES ('The Rao''s', 1);
-- Create gift
INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftPicture, GiftSize, CategoryID, GiftRating)
    VALUES (1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'default.png', 'N/A', 2, 5.0);
-- Add user to group
INSERT INTO groups_users (groups_users.GroupID, groups_users.UserID) VALUES (1, 1);
INSERT INTO groups_users (groups_users.GroupID, groups_users.UserID) VALUES (1, 2);
-- Add gift to group
INSERT INTO groups_gifts (GroupID, GiftID) VALUES (1, 1);
-- Make new year's visible to group
INSERT INTO events_users_groups (EventUserID, GroupID) VALUES (2, 1);
INSERT INTO events_users_groups (EventUserID, GroupID) VALUES (3, 1);