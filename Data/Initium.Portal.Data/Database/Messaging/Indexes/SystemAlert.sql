CREATE UNIQUE INDEX systemalert_id_uindex
    ON "Messaging"."SystemAlert" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;