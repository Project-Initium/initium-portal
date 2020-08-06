CREATE OR REPLACE VIEW "Portal"."vwRoleResource"
AS
SELECT rr."RoleId",
       rr."ResourceId"
FROM "AccessProtection"."RoleResource" rr;

ALTER TABLE "Portal"."vwRoleResource"
    OWNER TO postgres;