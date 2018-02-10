-- Create categories:
INSERT INTO gift_registry_db.categories (CategoryID, CategoryName, CategoryDescription)
    VALUES 
        (1, 'Clothing', 'Clothing'),
        (2, 'Electronics', 'Electronic Devices');

-- Create users:
INSERT INTO gift_registry_db.users (UserID, UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserFacebookID, UserGoogleID)
    VALUES 
        (1, 'Alex Rao', 'alexhrao@gmail.com', 7, 3, "I LOVE to have fun!", "UJIOl8xUU7ccIQH5Ofs0Awfdsa00", "12345", "12345"),
        (2, 'Raeedah Choudhury', 'raeedah.choudhury@gmail.com', 11, 19, "I love my boyfriend", "UJIOl8xUU7ccIQH5Ofs0Awfdsa11", NULL, NULL),
        (3, 'Alex Tes', 'alexhrao@gatech.edu', 7, 3, "fdsafdsa", "UJIOl7xUU7ccIdH5Ofs0Awfdsa00", NULL, NULL),
        (4, 'Alex Tes', 'arao81@gatech.edu', 7, 3, "fdsafdsa", "UqIOl7xfU7ccIQH5Ofs0Awfdsa00", NULL, NULL),
        (5, 'ToDelete', 'alexhrao@hotmail.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd8765", "54321", "54321"),
        (6, 'NoImage', 'alexhrao@github.edu', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd1234", "43215", "43215"),
        (7, 'HasImage', 'alexhrao@google.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5Ofs0Awfd2222", "12312", "12312"),
        (8, 'reservations', 'asdf@google.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5OfdfAwfd2222", "55555", "55555"),
        (9, 'reservationsHas', 'asdffdsasadf@google.com', 7, 3, "fdsafdsa", "UaIOl0xfU7ccIQH5OfdfAw555222", "66", "1234");
-- Create preferences:
INSERT INTO gift_registry_db.preferences (UserID, UserCulture, UserTheme)
    VALUES 
        (1, 'en-US', 0),
        (2, 'fr-FR', 1);
    
-- Create Password:
INSERT INTO gift_registry_db.passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES 
        (1, 'xITVQ4AJ9eLfuzeG+cxeajH3QRw=', 'lN6ivSpluwewZWZCVXFNBw==', 10000),
        (2, '12345678901234567890', '1234567890123567', 10000);

-- Create Events
INSERT INTO gift_registry_db.user_events (EventID, UserID, EventStartDate, EventEndDate, EventName)
    VALUES 
        (1, 1, '2017-01-10', NULL, 'Exact Event Test'),
        (2, 1, '2017-01-7', '2020-01-01', 'Relative Event Test'),
        (3, 1, '2017-08-04', '2017-08-04', 'One Off Event Test'),
        (4, 1, '2017-01-05', NULL, 'Blackout Date Test'),
        (5, 2, '2017-01-10', NULL, 'Exact Event Test 2'),
        (6, 2, '2017-01-7', '2020-01-01', 'Relative Event Test 2'),
        (7, 2, '2017-08-04', '2017-08-04', 'One Off Event Test 2'),
        (8, 2, '2017-04-04', NULL, 'Blackout Date Test 2');

-- Create groups
INSERT INTO gift_registry_db.groups (GroupID, GroupName, AdminID)
    VALUES
        (1, 'The Rao''s', 1),
        (2, 'Test Group 2', 2),
        (3, 'Test Group 3', 4),
        (4, 'Test Group 1 is out', 4);
    
-- Create gift
INSERT INTO gift_registry_db.gifts (GiftID, UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating)
    VALUES 
        (1, 1, 'Webcam', 'Logitech BRIO webcam', 'https://www.google.com', 100.00, 'Logitech, Target', 1, 'FFFFFF', 'White', 'N/A', 2, 5.0),
        (2, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1000, '000000', 'black', 'BIG', 1, 5.0),
        (3, 1, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 3, '000000', 'black', 'BIG', 1, 5.0),
        (4, 7, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 5, '000000', 'black', 'BIG', 1, 5.0),
        (5, 2, 'Wafdsav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0),
        (6, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0),
        (7, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 5600, '000000', 'black', 'BIG', 1, 5.0),
        (8, 2, 'Wav', 'WavDeWav', 'https://www.google.com', 100000.00, 'Target, Target', 1, '000000', 'black', 'BIG', 1, 5.0);

-- Add user to group
INSERT INTO gift_registry_db.groups_users (GroupID, UserID, IsChild)
    VALUES (1, 2, TRUE);
INSERT INTO gift_registry_db.groups_users (GroupID, UserID)
    VALUES 
        (2, 3),
        (1, 1),
        (2, 1),
        (2, 5),
        (3, 1),
        (3, 2),
        (4, 2),
        (4, 3),
        (4, 6);

-- Add gift to group
INSERT INTO gift_registry_db.groups_gifts (GroupID, GiftID)
    VALUES 
        (1, 1),
        (1, 2);

-- Create the Exact Event Rule
INSERT INTO gift_registry_db.exact_events (EventID, EventTimeInterval, EventSkipEvery)
    VALUES
        (1, 'M', 1),
        (5, 'W', 3),
        (4, 'M', 1);
    
-- Create the Relative Event Rule
INSERT INTO gift_registry_db.relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn)
    VALUES 
        (2, 'MAR', 1, 'M', 2),
        (6, NULL, 2, 'S', 3),
        (8, NULL, 1, 'T', 5);

-- Create Blackout Dates
INSERT INTO gift_registry_db.event_blackouts (EventID, EventBlackoutDate)
    VALUES
        (4, '2017-02-05'),
        (8, '2017-06-27');

-- Add events to groups
INSERT INTO gift_registry_db.groups_events (GroupID, EventID)
    VALUES
        (1, 1),
        (1, 2),
        (1, 3),
        (2, 4),
        (1, 5),
        (2, 6),
        (2, 7),
        (4, 8);
    
-- Add Reservations
INSERT INTO gift_registry_db.reservations (ReservationID, GiftID, UserID, PurchaseStamp)
    VALUES
        ( 1, 6, 1, NULL),
        ( 2, 3, 1, '2017-03-09'),
        ( 3, 3, 1, NULL),
        ( 4, 3, 1, NULL),
        ( 5, 8, 2, '2017-08-09'),
        ( 6, 7, 1, NULL),
        ( 7, 5, 4, NULL),
        ( 8, 5, 4, NULL),
        ( 9, 5, 4, NULL),
        (10, 5, 4, NULL),
        (11, 5, 4, NULL),
        (12, 7, 6, NULL),
        (13, 2, 3, NULL),
        (14, 2, 3, NULL),
        (15, 2, 3, NULL),
        (16, 2, 7, NULL),
        (17, 7, 2, '2017-08-09'),
        (18, 7, 2, '2017-08-10'),
        (19, 7, 2, '2017-08-09'),
        (20, 7, 2, '2017-08-14'),
        (21, 2, 1, '2017-02-02'),
        (22, 2, 1, '2017-06-02'),
        (23, 2, 1, '2017-04-02'),
        (24, 7, 9, NULL);
        
-- Add Cultures
INSERT INTO gift_registry_db.cultures (CultureID, CultureLanguage, CultureLocation, CultureName, CultureDesc)
    VALUES
        (1, 'en', 'US', 'English (US)', 'English (United States)'),
        (2, 'fr', 'FR', 'French (FR)', 'French'),
        (3, 'en', 'GB', 'English (UK)', 'English (United Kingdom)');