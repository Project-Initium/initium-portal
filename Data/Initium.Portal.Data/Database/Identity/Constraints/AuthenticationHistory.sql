ALTER TABLE "Identity"."AuthenticationHistory" ADD CONSTRAINT authenticationhistory_user_id_fk FOREIGN KEY ("UserId")
    REFERENCES "Identity"."User" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;