CREATE TABLE "Messaging"."UserNotification"
(
    "NotificationId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "WhenViewed" timestamp without time zone,
    "WhenDismissed" timestamp without time zone,
    CONSTRAINT usernotification_pk PRIMARY KEY ("NotificationId", "UserId")
)

TABLESPACE pg_default;

ALTER TABLE "Messaging"."UserNotification"
    OWNER to postgres;