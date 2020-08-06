CREATE UNIQUE INDEX role_id_uindex
    ON "AccessProtection"."Role" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;