CREATE UNIQUE INDEX authenticationhistory_id_uindex
    ON "Identity"."AuthenticationHistory" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;