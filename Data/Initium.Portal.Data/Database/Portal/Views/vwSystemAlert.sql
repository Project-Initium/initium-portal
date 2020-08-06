CREATE OR REPLACE VIEW "Portal"."vwSystemAlert"
AS
SELECT sa."Id",
       sa."Name",
       sa."Message",
       sa."Type",
       sa."WhenToShow",
       sa."WhenToHide",
       sa."IsActive"
FROM ( SELECT "SystemAlert"."Id",
              "SystemAlert"."Name",
              "SystemAlert"."Message",
              "SystemAlert"."Type",
              "SystemAlert"."WhenToShow",
              "SystemAlert"."WhenToHide",
              CASE
                  WHEN "SystemAlert"."WhenToShow" IS NULL AND "SystemAlert"."WhenToHide" IS NULL THEN true
                  WHEN "SystemAlert"."WhenToShow" < timezone('utc'::text, now()) AND "SystemAlert"."WhenToHide" IS NULL THEN true
                  WHEN "SystemAlert"."WhenToShow" IS NULL AND "SystemAlert"."WhenToHide" > timezone('utc'::text, now()) THEN true
                  WHEN "SystemAlert"."WhenToShow" > timezone('utc'::text, now()) AND "SystemAlert"."WhenToHide" < timezone('utc'::text, now()) THEN true
                  ELSE false
                  END AS "IsActive"
       FROM "Messaging"."SystemAlert") sa;

ALTER TABLE "Portal"."vwSystemAlert"
    OWNER TO postgres;
