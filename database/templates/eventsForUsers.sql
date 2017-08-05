USE gift_registry_db;
SELECT eventsusersgroups.EventUserID
FROM eventsusersgroups
INNER JOIN eventsusers ON eventsusersgroups.EventUserID = eventsusers.EventUserID
WHERE eventsusers.UserID IN (
    SELECT UserID
    FROM gift_registry_db.groupsusers
    WHERE GroupID IN (
        SELECT GroupID
        FROM gift_registry_db.groupsusers
        WHERE gift_registry_db.groupsusers.UserID = 1
    )
)
AND eventsusersgroups.GroupID IN (
    SELECT GroupID
    FROM gift_registry_db.groupsusers
    WHERE GroupID IN (
        SELECT GroupID
        FROM gift_registry_db.groupsusers
        WHERE gift_registry_db.groupsusers.UserID = 1
    )
);