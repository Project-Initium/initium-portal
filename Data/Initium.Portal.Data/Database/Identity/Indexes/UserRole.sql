CREATE UNIQUE INDEX userrole_roleid_uindex
    ON "Identity"."UserRole" USING btree
        ("RoleId" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE UNIQUE INDEX userrole_userid_uindex
    ON "Identity"."UserRole" USING btree
        ("UserId" ASC NULLS LAST)
    TABLESPACE pg_default;