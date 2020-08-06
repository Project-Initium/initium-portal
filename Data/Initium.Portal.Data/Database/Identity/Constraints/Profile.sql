ALTER TABLE "Identity"."Profile" ADD CONSTRAINT profile_user_id_fk FOREIGN KEY ("UserId")
    REFERENCES "Identity"."User" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION