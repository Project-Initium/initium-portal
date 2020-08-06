

ALTER TABLE "Identity"."UserRole" ADD CONSTRAINT userrole_role_id_fk FOREIGN KEY ("RoleId")
        REFERENCES "AccessProtection"."Role" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION;

ALTER TABLE "Identity"."UserRole" ADD CONSTRAINT userrole_user_id_fk FOREIGN KEY ("UserId")
        REFERENCES "Identity"."User" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION