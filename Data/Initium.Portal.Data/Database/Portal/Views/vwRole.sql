CREATE OR REPLACE VIEW "Portal"."vwRole"
AS
SELECT r."Id",
       r."Name",
       count(rr."RoleId") AS resourcecount,
       count(ur."RoleId") AS usercount
FROM "AccessProtection"."Role" r
         LEFT JOIN "AccessProtection"."RoleResource" rr ON rr."RoleId" = r."Id"
         LEFT JOIN "Identity"."UserRole" ur ON ur."RoleId" = r."Id"
GROUP BY r."Id", r."Name";

ALTER TABLE "Portal"."vwRole"
    OWNER TO postgres;
