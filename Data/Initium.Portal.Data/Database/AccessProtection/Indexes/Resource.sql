CREATE UNIQUE INDEX resource_name_uindex
    ON "AccessProtection"."Resource" USING btree
        ("Name" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: resource_normalizedname_uindex

-- DROP INDEX "AccessProtection".resource_normalizedname_uindex;

CREATE UNIQUE INDEX resource_normalizedname_uindex
    ON "AccessProtection"."Resource" USING btree
        ("NormalizedName" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;