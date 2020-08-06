CREATE UNIQUE INDEX notification_id_uindex
    ON "Messaging"."Notification" USING btree
        ("Id" ASC NULLS LAST)
    TABLESPACE pg_default;