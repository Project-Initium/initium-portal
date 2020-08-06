CREATE TABLE "AccessProtection"."RoleResource"
(
    "RoleId" uuid NOT NULL,
    "ResourceId" uuid NOT NULL,
    CONSTRAINT roleresource_pk PRIMARY KEY ("RoleId", "ResourceId")
)

TABLESPACE pg_default;

ALTER TABLE "AccessProtection"."RoleResource"
    OWNER to postgres;