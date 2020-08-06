CREATE TABLE "Identity"."UserRole"
(
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    CONSTRAINT userrole_pk PRIMARY KEY ("RoleId", "UserId")
)

TABLESPACE pg_default;

ALTER TABLE "Identity"."UserRole"
    OWNER to postgres;