CREATE OR REPLACE VIEW "Messaging"."vwUserNotification"
AS
SELECT un."UserId",
       un."NotificationId"
FROM "Messaging"."UserNotification" un;

ALTER TABLE "Messaging"."vwUserNotification"
    OWNER TO postgres;
