CREATE TABLE "Identity"."SecurityTokenMapping"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Purpose" integer NOT NULL,
    "WhenCreated" timestamp(6) without time zone,
    "WhenExpires" timestamp(6) without time zone NOT NULL,
    "WhenUsed" timestamp(6) without time zone,
    CONSTRAINT securitytokenmapping_pk PRIMARY KEY ("Id")
)

    TABLESPACE pg_default;

ALTER TABLE "Identity"."SecurityTokenMapping"
    OWNER to postgres;