CREATE TABLE "AccessProtection"."Resource"
(
    "Id" uuid NOT NULL,
    "Name" character varying(100) COLLATE pg_catalog."default" NOT NULL,
    "NormalizedName" character varying(100) COLLATE pg_catalog."default" NOT NULL,
    "ParentResourceId" uuid,
    CONSTRAINT resource_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "AccessProtection"."Resource"
    OWNER to postgres;