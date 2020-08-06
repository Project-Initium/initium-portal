CREATE OR REPLACE VIEW "Portal"."vwUserNotification"
AS
SELECT un."NotificationId",
       un."UserId",
       n."WhenNotified",
       n."Type",
       n."SerializedEventData",
       n."Subject",
       n."Message",
       un."WhenViewed"
FROM "Messaging"."UserNotification" un
         JOIN "Messaging"."Notification" n ON un."NotificationId" = n."Id"
WHERE un."WhenDismissed" IS NULL;

ALTER TABLE "Portal"."vwUserNotification"
    OWNER TO postgres;
