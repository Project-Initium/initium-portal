CREATE UNIQUE INDEX profile_userid_uindex
    ON "Identity"."Profile" USING btree
        ("UserId" ASC NULLS LAST)
    TABLESPACE pg_default;