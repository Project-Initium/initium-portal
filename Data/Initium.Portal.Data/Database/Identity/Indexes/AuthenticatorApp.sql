CREATE UNIQUE INDEX authenticatorapp_id_uindex
    ON "Identity"."AuthenticatorApp" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;