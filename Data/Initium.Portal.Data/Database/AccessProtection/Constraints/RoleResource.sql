ALTER TABLE "AccessProtection"."RoleResource" ADD CONSTRAINT roleresource_resource_id_fk FOREIGN KEY ("ResourceId")
        REFERENCES "AccessProtection"."Resource" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION;

ALTER TABLE "AccessProtection"."RoleResource" ADD CONSTRAINT roleresource_role_id_fk FOREIGN KEY ("RoleId")
        REFERENCES "AccessProtection"."Role" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION;