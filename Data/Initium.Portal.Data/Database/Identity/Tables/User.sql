CREATE TABLE "Identity"."User"
(
    "Id" uuid NOT NULL,
    "EmailAddress" character varying(320) COLLATE pg_catalog."default" NOT NULL,
    "PasswordHash" character varying(100) COLLATE pg_catalog."default" NOT NULL,
    "WhenCreated" timestamp without time zone NOT NULL,
    "WhenLastAuthenticated" timestamp without time zone,
    "IsLockable" boolean NOT NULL DEFAULT false,
    "WhenLocked" timestamp without time zone,
    "AttemptsSinceLastAuthentication" integer NOT NULL DEFAULT 0,
    "SecurityStamp" uuid NOT NULL,
    "IsAdmin" boolean NOT NULL DEFAULT false,
    "WhenVerified" timestamp without time zone,
    "WhenDisabled" timestamp without time zone,
    CONSTRAINT user_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "Identity"."User"
    OWNER to postgres;