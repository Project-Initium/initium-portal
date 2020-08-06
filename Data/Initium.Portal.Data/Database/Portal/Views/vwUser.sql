CREATE OR REPLACE VIEW "Portal"."vwUser"
AS
SELECT u."Id",
       u."EmailAddress",
       u."IsLockable",
       CASE
           WHEN u."WhenLocked" IS NULL THEN false
           ELSE true
           END AS islocked,
       u."WhenLastAuthenticated",
       p."FirstName",
       p."LastName",
       u."WhenCreated",
       CASE
           WHEN u."WhenVerified" IS NULL THEN false
           ELSE true
           END AS isverified,
       u."IsAdmin",
       u."WhenLocked",
       u."WhenDisabled"
FROM "Identity"."User" u
         LEFT JOIN "Identity"."Profile" p ON u."Id" = p."UserId";

ALTER TABLE "Portal"."vwUser"
    OWNER TO postgres;