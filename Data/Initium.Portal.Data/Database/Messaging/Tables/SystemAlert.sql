CREATE TABLE "Messaging"."SystemAlert"
(
    "Id" uuid NOT NULL,
    "Name" text COLLATE pg_catalog."default" NOT NULL,
    "Message" text COLLATE pg_catalog."default" NOT NULL,
    "Type" integer NOT NULL,
    "WhenToShow" timestamp without time zone,
    "WhenToHide" timestamp without time zone,
    CONSTRAINT systemalert_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "Messaging"."SystemAlert"
    OWNER to postgres;