CREATE TABLE gift_registry_db.gifts (
    GiftID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    GiftName VARCHAR(255) NOT NULL,
    GiftDescription VARCHAR(8191) NULL,
    GiftURL VARCHAR(1023) NULL,
    GiftCost DECIMAL(15, 2) NULL,
    GiftStores VARCHAR(4095) NULL,
    GiftQuantity INT UNSIGNED NOT NULL DEFAULT 0,
    GiftPicture VARCHAR(255) NULL,
    GiftSize VARCHAR(127) NULL,
    CategoryID INT NOT NULL,
    GiftRating DECIMAL(3, 2) NOT NULL DEFAULT 0,
    GiftAddDate DATE NULL,
    GiftReceivedDate DATE NULL
);

ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsUsers FOREIGN KEY (UserID)
        REFERENCES gift_registry_db.users(UserID);

ALTER TABLE gift_registry_db.gifts
    ADD CONSTRAINT FK_GiftsCategories FOREIGN KEY (CategoryID)
        REFERENCES gift_registry_db.categories(CategoryID);