CREATE OR REPLACE VIEW "Portal"."vwResource"
AS
SELECT r."Id",
       r."Name",
       r."NormalizedName",
       r."ParentResourceId"
FROM "AccessProtection"."Resource" r;

ALTER TABLE "Portal"."vwResource"
    OWNER TO postgres;