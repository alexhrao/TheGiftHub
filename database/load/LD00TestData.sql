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
INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio)
    VALUES ('Alex Rao', 'alexhrao@gmail.com', 7, 3, "I LOVE to have fun!");
INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio)
    VALUES ('Raeedah Choudhury', 'rchoudhury@gmail.com', 11, 19, "I love my boyfriend");

-- Insert into preferences:
INSERT INTO preferences (UserID, UserLanguage, UserLocation, UserTheme)
    VALUES (1, 'en', 'US', 0);
INSERT INTO preferences (UserID, UserLanguage, UserLocation, UserTheme)
    VALUES (2, 'fr', 'FR', 1);
    
-- Insert into Password:
INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (1, 'UJIOl8xUU7ccIQH5Ofs+Awfdsa==', 'UJIOl8xUU7ccIQH5Ofs+Aw==', 10000);
INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter)
    VALUES (1, '12345678901234567890', '1234567890123567', 10000);

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

-- Add Languages
INSERT INTO languages (LanguageCode, LanguageName)
    VALUES
    ('en', 'English'),
    ('fr', 'French'),
    ('es', 'Spanish');

-- Add Countries
INSERT INTO countries (CountryCode, CountryName)
    VALUES
    ('AD', 'Andorra'),
    ('AE', 'United Arab Emirates'),
    ('AF', 'Afghanistan'),
    ('AG', 'Antigua and Barbuda'),
    ('AI', 'Anguilla'),
    ('AL', 'Albania'),
    ('AM', 'Armenia'),
    ('AO', 'Angola'),
    ('AQ', 'Antarctica'),
    ('AR', 'Argentina'),
    ('AS', 'American Samoa'),
    ('AT', 'Austria'),
    ('AU', 'Australia'),
    ('AW', 'Aruba'),
    ('AX', 'Åland'),
    ('AZ', 'Azerbaijan'),
    ('BA', 'Bosnia and Herzegovina'),
    ('BB', 'Barbados'),
    ('BD', 'Bangladesh'),
    ('BE', 'Belgium'),
    ('BF', 'Burkina Faso'),
    ('BG', 'Bulgaria'),
    ('BH', 'Bahrain'),
    ('BI', 'Burundi'),
    ('BJ', 'Benin'),
    ('BL', 'Saint Barthélemy'),
    ('BM', 'Bermuda'),
    ('BN', 'Brunei'),
    ('BO', 'Bolivia'),
    ('BQ', 'Bonaire'),
    ('BR', 'Brazil'),
    ('BS', 'Bahamas'),
    ('BT', 'Bhutan'),
    ('BV', 'Bouvet Island'),
    ('BW', 'Botswana'),
    ('BY', 'Belarus'),
    ('BZ', 'Belize'),
    ('CA', 'Canada'),
    ('CC', 'Cocos [Keeling] Islands'),
    ('CD', 'Democratic Republic of the Congo'),
    ('CF', 'Central African Republic'),
    ('CG', 'Republic of the Congo'),
    ('CH', 'Switzerland'),
    ('CI', 'Ivory Coast'),
    ('CK', 'Cook Islands'),
    ('CL', 'Chile'),
    ('CM', 'Cameroon'),
    ('CN', 'China'),
    ('CO', 'Colombia'),
    ('CR', 'Costa Rica'),
    ('CU', 'Cuba'),
    ('CV', 'Cape Verde'),
    ('CW', 'Curacao'),
    ('CX', 'Christmas Island'),
    ('CY', 'Cyprus'),
    ('CZ', 'Czechia'),
    ('DE', 'Germany'),
    ('DJ', 'Djibouti'),
    ('DK', 'Denmark'),
    ('DM', 'Dominica'),
    ('DO', 'Dominican Republic'),
    ('DZ', 'Algeria'),
    ('EC', 'Ecuador'),
    ('EE', 'Estonia'),
    ('EG', 'Egypt'),
    ('EH', 'Western Sahara'),
    ('ER', 'Eritrea'),
    ('ES', 'Spain'),
    ('ET', 'Ethiopia'),
    ('FI', 'Finland'),
    ('FJ', 'Fiji'),
    ('FK', 'Falkland Islands'),
    ('FM', 'Micronesia'),
    ('FO', 'Faroe Islands'),
    ('FR', 'France'),
    ('GA', 'Gabon'),
    ('GB', 'United Kingdom'),
    ('GD', 'Grenada'),
    ('GE', 'Georgia'),
    ('GF', 'French Guiana'),
    ('GG', 'Guernsey'),
    ('GH', 'Ghana'),
    ('GI', 'Gibraltar'),
    ('GL', 'Greenland'),
    ('GM', 'Gambia'),
    ('GN', 'Guinea'),
    ('GP', 'Guadeloupe'),
    ('GQ', 'Equatorial Guinea'),
    ('GR', 'Greece'),
    ('GS', 'South Georgia and the South Sandwich Islands'),
    ('GT', 'Guatemala'),
    ('GU', 'Guam'),
    ('GW', 'Guinea-Bissau'),
    ('GY', 'Guyana'),
    ('HK', 'Hong Kong'),
    ('HM', 'Heard Island and McDonald Islands'),
    ('HN', 'Honduras'),
    ('HR', 'Croatia'),
    ('HT', 'Haiti'),
    ('HU', 'Hungary'),
    ('ID', 'Indonesia'),
    ('IE', 'Ireland'),
    ('IL', 'Israel'),
    ('IM', 'Isle of Man'),
    ('IN', 'India'),
    ('IO', 'British Indian Ocean Territory'),
    ('IQ', 'Iraq'),
    ('IR', 'Iran'),
    ('IS', 'Iceland'),
    ('IT', 'Italy'),
    ('JE', 'Jersey'),
    ('JM', 'Jamaica'),
    ('JO', 'Jordan'),
    ('JP', 'Japan'),
    ('KE', 'Kenya'),
    ('KG', 'Kyrgyzstan'),
    ('KH', 'Cambodia'),
    ('KI', 'Kiribati'),
    ('KM', 'Comoros'),
    ('KN', 'Saint Kitts and Nevis'),
    ('KP', 'North Korea'),
    ('KR', 'South Korea'),
    ('KW', 'Kuwait'),
    ('KY', 'Cayman Islands'),
    ('KZ', 'Kazakhstan'),
    ('LA', 'Laos'),
    ('LB', 'Lebanon'),
    ('LC', 'Saint Lucia'),
    ('LI', 'Liechtenstein'),
    ('LK', 'Sri Lanka'),
    ('LR', 'Liberia'),
    ('LS', 'Lesotho'),
    ('LT', 'Lithuania'),
    ('LU', 'Luxembourg'),
    ('LV', 'Latvia'),
    ('LY', 'Libya'),
    ('MA', 'Morocco'),
    ('MC', 'Monaco'),
    ('MD', 'Moldova'),
    ('ME', 'Montenegro'),
    ('MF', 'Saint Martin'),
    ('MG', 'Madagascar'),
    ('MH', 'Marshall Islands'),
    ('MK', 'Macedonia'),
    ('ML', 'Mali'),
    ('MM', 'Myanmar [Burma]'),
    ('MN', 'Mongolia'),
    ('MO', 'Macao'),
    ('MP', 'Northern Mariana Islands'),
    ('MQ', 'Martinique'),
    ('MR', 'Mauritania'),
    ('MS', 'Montserrat'),
    ('MT', 'Malta'),
    ('MU', 'Mauritius'),
    ('MV', 'Maldives'),
    ('MW', 'Malawi'),
    ('MX', 'Mexico'),
    ('MY', 'Malaysia'),
    ('MZ', 'Mozambique'),
    ('NA', 'Namibia'),
    ('NC', 'New Caledonia'),
    ('NE', 'Niger'),
    ('NF', 'Norfolk Island'),
    ('NG', 'Nigeria'),
    ('NI', 'Nicaragua'),
    ('NL', 'Netherlands'),
    ('NO', 'Norway'),
    ('NP', 'Nepal'),
    ('NR', 'Nauru'),
    ('NU', 'Niue'),
    ('NZ', 'New Zealand'),
    ('OM', 'Oman'),
    ('PA', 'Panama'),
    ('PE', 'Peru'),
    ('PF', 'French Polynesia'),
    ('PG', 'Papua New Guinea'),
    ('PH', 'Philippines'),
    ('PK', 'Pakistan'),
    ('PL', 'Poland'),
    ('PM', 'Saint Pierre and Miquelon'),
    ('PN', 'Pitcairn Islands'),
    ('PR', 'Puerto Rico'),
    ('PS', 'Palestine'),
    ('PT', 'Portugal'),
    ('PW', 'Palau'),
    ('PY', 'Paraguay'),
    ('QA', 'Qatar'),
    ('RE', 'Réunion'),
    ('RO', 'Romania'),
    ('RS', 'Serbia'),
    ('RU', 'Russia'),
    ('RW', 'Rwanda'),
    ('SA', 'Saudi Arabia'),
    ('SB', 'Solomon Islands'),
    ('SC', 'Seychelles'),
    ('SD', 'Sudan'),
    ('SE', 'Sweden'),
    ('SG', 'Singapore'),
    ('SH', 'Saint Helena'),
    ('SI', 'Slovenia'),
    ('SJ', 'Svalbard and Jan Mayen'),
    ('SK', 'Slovakia'),
    ('SL', 'Sierra Leone'),
    ('SM', 'San Marino'),
    ('SN', 'Senegal'),
    ('SO', 'Somalia'),
    ('SR', 'Suriname'),
    ('SS', 'South Sudan'),
    ('ST', 'São Tomé and Príncipe'),
    ('SV', 'El Salvador'),
    ('SX', 'Sint Maarten'),
    ('SY', 'Syria'),
    ('SZ', 'Swaziland'),
    ('TC', 'Turks and Caicos Islands'),
    ('TD', 'Chad'),
    ('TF', 'French Southern Territories'),
    ('TG', 'Togo'),
    ('TH', 'Thailand'),
    ('TJ', 'Tajikistan'),
    ('TK', 'Tokelau'),
    ('TL', 'East Timor'),
    ('TM', 'Turkmenistan'),
    ('TN', 'Tunisia'),
    ('TO', 'Tonga'),
    ('TR', 'Turkey'),
    ('TT', 'Trinidad and Tobago'),
    ('TV', 'Tuvalu'),
    ('TW', 'Taiwan'),
    ('TZ', 'Tanzania'),
    ('UA', 'Ukraine'),
    ('UG', 'Uganda'),
    ('UM', 'U.S. Minor Outlying Islands'),
    ('US', 'United States'),
    ('UY', 'Uruguay'),
    ('UZ', 'Uzbekistan'),
    ('VA', 'Vatican City'),
    ('VC', 'Saint Vincent and the Grenadines'),
    ('VE', 'Venezuela'),
    ('VG', 'British Virgin Islands'),
    ('VI', 'U.S. Virgin Islands'),
    ('VN', 'Vietnam'),
    ('VU', 'Vanuatu'),
    ('WF', 'Wallis and Futuna'),
    ('WS', 'Samoa'),
    ('XK', 'Kosovo'),
    ('YE', 'Yemen'),
    ('YT', 'Mayotte'),
    ('ZA', 'South Africa'),
    ('ZM', 'Zambia'),
    ('ZW', 'Zimbabwe');