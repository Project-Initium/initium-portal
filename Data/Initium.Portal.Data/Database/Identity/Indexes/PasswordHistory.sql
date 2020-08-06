CREATE UNIQUE INDEX passwordhistory_id_uindex
    ON "Identity"."PasswordHistory" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;