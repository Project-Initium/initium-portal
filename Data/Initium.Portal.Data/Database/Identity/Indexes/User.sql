CREATE UNIQUE INDEX user_emailaddress_uindex
    ON "Identity"."User" USING btree
        ("EmailAddress" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;