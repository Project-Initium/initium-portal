CREATE UNIQUE INDEX securitytokenmapping_id_uindex
    ON "Identity"."SecurityTokenMapping" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;