CREATE OR REPLACE VIEW "Portal"."vwUserRole"
AS
SELECT ur."RoleId",
       ur."UserId"
FROM "Identity"."UserRole" ur;

ALTER TABLE "Portal"."vwUserRole"
    OWNER TO postgres;

