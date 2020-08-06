CREATE TABLE "Messaging"."Notification"
(
    "Id" uuid NOT NULL,
    "Subject" text COLLATE pg_catalog."default" NOT NULL,
    "Message" text COLLATE pg_catalog."default" NOT NULL,
    "Type" integer NOT NULL,
    "SerializedEventData" text COLLATE pg_catalog."default" NOT NULL,
    "WhenNotified" timestamp without time zone NOT NULL,
    CONSTRAINT notification_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "Messaging"."Notification"
    OWNER to postgres;