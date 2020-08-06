CREATE TABLE "Identity"."Profile"
(
    "UserId" uuid NOT NULL,
    "FirstName" character varying(300) COLLATE pg_catalog."default",
    "LastName" character varying(300) COLLATE pg_catalog."default",
     CONSTRAINT profile_pk PRIMARY KEY ("UserId")
)

    TABLESPACE pg_default;

ALTER TABLE "Identity"."Profile"
    OWNER to postgres;