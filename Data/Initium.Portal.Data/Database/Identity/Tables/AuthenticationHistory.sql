CREATE TABLE "Identity"."AuthenticationHistory"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "WhenHappened" timestamp without time zone NOT NULL,
    "AuthenticationHistoryType" integer NOT NULL,
    CONSTRAINT authenticationhistory_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "Identity"."AuthenticationHistory"
    OWNER to postgres;