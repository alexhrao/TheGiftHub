-- Create categories:
INSERT INTO gift_registry_db_test.categories (CategoryID, CategoryName, CategoryDescription)
    VALUES (1, 'Clothing', 'Clothing');
INSERT INTO gift_registry_db_test.categories (CategoryID, CategoryName, CategoryDescription)
    VALUES (2, 'Electronics', 'Electronic Devices');

-- Create users:
INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES (1, 'Alex Rao', 'alexhrao@gmail.com', 7, 3, "I LOVE to have fun!", "UJIOl8xUU7ccIQH5Ofs0Awfdsa00");
INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES (2, 'Raeedah Choudhury', 'raeedah.choudhury@gmail.com', 11, 19, "I love my boyfriend", "UJIOl8xUU7ccIQH5Ofs0Awfdsa11");
INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES (3, 'Alex Tes', 'alexhrao@gatech.edu', 7, 3, "fdsafdsa", "UJIOl7xUU7ccIQH5Ofs0Awfdsa00");
INSERT INTO gift_registry_db_test.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL)
    VALUES (4, 'Alex Tes', 'arao81@gatech.edu', 7, 3, "fdsafdsa", "UJIOl7xfU7ccIQH5Ofs0Awfdsa00");
    
-- Create preferences:
INSERT INTO gift_registry_db_test.preferences (UserID, UserCulture, UserTheme)
    VALUES (1, 'en-US', 0);
INSERT INTO gift_registry_db_test.preferences (UserID, UserCulture, UserTheme)
    VALUES (2, 'fr-FR', 1);
    
-- Create Password:
INSERT INTO gift_registry_db_test.passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (1, 'xITVQ4AJ9eLfuzeG+cxeajH3QRw=', 'lN6ivSpluwewZWZCVXFNBw==', 10000);

INSERT INTO gift_registry_db_test.passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (2, '12345678901234567890', '1234567890123567', 10000);

-- Create Events
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (1, 1, '2017-01-10', NULL, 'Exact Event Test');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (2, 1, '2017-01-7', '2020-01-01', 'Relative Event Test');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (3, 1, '2017-08-04', '2017-08-04', 'One Off Event Test');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (4, 1, '2017-01-05', NULL, 'Blackout Date Test');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (5, 2, '2017-01-10', NULL, 'Exact Event Test 2');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (6, 2, '2017-01-7', '2020-01-01', 'Relative Event Test 2');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (7, 2, '2017-08-04', '2017-08-04', 'One Off Event Test 2');
INSERT INTO gift_registry_db_test.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
	VALUES (8, 2, '2017-04-04', NULL, 'Blackout Date Test 2');

-- Create groups
INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
    VALUES (1, 'The Rao''s', 1);
INSERT INTO gift_registry_db_test.groups (GroupID, GroupName, AdminID)
	VALUES (2, 'Test Group 2', 2);
    
-- Create gift
INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
    VALUES (1, 1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'FFFFFF', 'White', 'N/A', 2, 5.0);
INSERT INTO gift_registry_db_test.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
    VALUES (2, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1000, '000000', 'black', 'BIG', 1, 5.0);

-- Add user to group
INSERT INTO gift_registry_db_test.groups_users (GroupID, UserID)
    VALUES (1, 1);
INSERT INTO gift_registry_db_test.groups_users (GroupID, UserID, IsChild)
    VALUES (1, 2, TRUE);

-- Add gift to group
INSERT INTO gift_registry_db_test.groups_gifts (GroupID, GiftID)
    VALUES (1, 1);
INSERT INTO gift_registry_db_test.groups_gifts (GroupID, GiftID)
    VALUES (1, 2);

-- Create the Exact Event Rule
INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
	VALUES (1, 'M', 1);
INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
	VALUES (5, 'W', 3);
INSERT INTO gift_registry_db_test.exact_events (EventID, EventTimeInterval, EventSkipEvery)
	VALUES (4, 'M', 1);
    
-- Create the Relative Event Rule
INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
	VALUES (2, 'MAR', 1, 'M', 2);
INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
	VALUES (6, NULL, 2, 'S', 3);
INSERT INTO gift_registry_db_test.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
	VALUES (8, NULL, 1, 'T', 5);

-- Create Blackout Dates
INSERT INTO gift_registry_db_test.event_blackouts (EventID, EventBlackoutDate)
	VALUES (4, '2017-02-05');
INSERT INTO gift_registry_db_test.event_blackouts (EventID, EventBlackoutDate)
	VALUES (8, '2017-06-27');

-- Add events to groups
INSERT INTO gift_registry_db_test.groups_events (GroupID, EventID)
	VALUES
		(1, 1),
        (1, 2),
        (1, 3),
        (2, 4),
        (1, 5),
        (2, 6),
        (2, 7),
        (2, 8);
    
-- Add Cultures
INSERT INTO gift_registry_db_test.cultures (CultureID, CultureLanguage, CultureLocation, CultureDesc)
    VALUES
    (1, 'en', 'US', 'English (United States)'),
    (2, 'fr', 'FR', 'French'),
    (3, 'en', 'GB', 'English (United Kingdom)');