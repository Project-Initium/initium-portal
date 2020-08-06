CREATE TABLE "Identity"."PasswordHistory"
(
    "Id" uuid NOT NULL,
    "Hash" character varying(100) COLLATE pg_catalog."default" NOT NULL,
    "WhenUsed" timestamp(6) without time zone,
    "UserId" uuid NOT NULL,
    CONSTRAINT passwordhistory_pk PRIMARY KEY ("Id")
)

    TABLESPACE pg_default;

ALTER TABLE "Identity"."PasswordHistory"
    OWNER to postgres;