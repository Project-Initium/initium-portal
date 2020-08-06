CREATE UNIQUE INDEX authenticatordevice_id_uindex
    ON "Identity"."AuthenticatorDevice" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;