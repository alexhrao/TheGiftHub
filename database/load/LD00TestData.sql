USE gift_registry_db;

-- Insert into categories:
INSERT INTO categories (CategoryName, CategoryDescription)
    VALUES ('Clothing', 'Clothing');
INSERT INTO categories (CategoryName, CategoryDescription)
    VALUES ('Electronics', 'Electronic Devices');

-- Default Events:
INSERT INTO default_events (EventName, EventYear, EventMonth, EventDay, EventRecurs)
    VALUES ('Christmas', 2017, 12, 25, TRUE);
INSERT INTO default_events (EventName, EventYear, EventMonth, EventDay, EventRecurs)
    VALUES ('New Year''s', 2017, 1, 1, TRUE);
INSERT INTO default_events (EventName, EventYear, EventMonth, EventDay, EventRecurs)
    VALUES ('Mother''s Day', 2017, 5, 8, FALSE);

-- Insert into users:
INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES ('Alex Rao', 'alexhrao@gmail.com', 7, 3, "I LOVE to have fun!", "UJIOl8xUU7ccIQH5Ofs0Awfdsa00");
INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES ('Raeedah Choudhury', 'rchoudhury@gmail.com', 11, 19, "I love my boyfriend", "UJIOl8xUU7ccIQH5Ofs0Awfdsa11");

-- Insert into preferences:
INSERT INTO preferences (UserID, UserCulture, UserTheme)
    VALUES (1, 'en-US', 0);
INSERT INTO preferences (UserID, UserCulture, UserTheme)
    VALUES (2, 'fr-FR', 1);
    
-- Insert into Password:
INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (1, 'UJIOl8xUU7ccIQH5Ofs+Awfdsa==', 'UJIOl8xUU7ccIQH5Ofs+Aw==', 10000);
INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (2, '12345678901234567890', '1234567890123567', 10000);

-- Default Event Futures:
INSERT INTO default_events_futures (EventID, EventYear, EventMonth, EventDay)
    VALUES (3, 2018, 5, 13);
INSERT INTO default_events_futures (EventID, EventYear, EventMonth, EventDay)
    VALUES (3, 2019, 5, 12);

-- Insert into events:
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventID, EventRecurs)
    VALUES ('Christmas', 1, 2017, 12, 25, 1, TRUE);
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventID, EventRecurs)
    VALUES ('New Year''s', 2, 2017, 1, 1, 2, TRUE);
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventID, EventRecurs)
    VALUES ('Christmas', 2, 2017, 12, 25, 1, TRUE);
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventID, EventRecurs)
    VALUES ('Mother''s Day', 2, 2017, 5, 8, 3, FALSE);
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventRecurs)
    VALUES ('Anniversary', 1, 2016, 10, 7, TRUE);
INSERT INTO events_users (EventName, UserID, EventYear, EventMonth, EventDay, EventRecurs)
    VALUES ('Anniversary', 2, 2016, 11, 9, FALSE);

-- Insert into events_futures
INSERT INTO events_users_futures (EventUserID, EventYear, EventMonth, EventDay)
    VALUES (6, 2017, 12, 1);
INSERT INTO events_users_futures (EventUserID, EventYear, EventMonth, EventDay)
    VALUES (6, 2019, 12, 1);

-- Create group
INSERT INTO groups (GroupName, AdminID)
    VALUES ('The Rao''s', 1);

-- Create gift
INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
    VALUES (1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'FFFFFF', 'White', 'N/A', 2, 5.0);
INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
    VALUES (2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1000, '000000', 'black', 'BIG', 1, 5.0);

-- Add user to group
INSERT INTO groups_users (GroupID, UserID)
    VALUES (1, 1);
INSERT INTO groups_users (GroupID, UserID, IsChild)
    VALUES (1, 2, TRUE);

-- Add gift to group
INSERT INTO groups_gifts (GroupID, GiftID)
    VALUES (1, 1);
INSERT INTO groups_gifts (GroupID, GiftID)
    VALUES (1, 2);
    
-- Make new year's visible to group
INSERT INTO events_users_groups (EventUserID, GroupID)
    VALUES (2, 1);
INSERT INTO events_users_groups (EventUserID, GroupID)
    VALUES (3, 1);

-- Add Cultures
INSERT INTO cultures (CultureLanguage, CultureLocation, CultureDesc)
    VALUES
    ('en', 'US', 'English (United States)'),
    ('fr', 'FR', 'French'),
    ('en', 'GB', 'English (United Kingdom)');