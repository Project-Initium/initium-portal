CREATE TABLE "AccessProtection"."Role"
(
    "Id" uuid NOT NULL,
    "Name" character varying(100) COLLATE pg_catalog."default",
    CONSTRAINT role_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "AccessProtection"."Role"
    OWNER to postgres;