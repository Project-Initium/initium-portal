CREATE TABLE "Identity"."AuthenticatorApp"
(
    "Id" uuid NOT NULL,
    "Key" character varying(500) COLLATE pg_catalog."default" NOT NULL,
    "WhenEnrolled" timestamp without time zone NOT NULL,
    "WhenRevoked" timestamp without time zone,
    "UserId" uuid NOT NULL,
    CONSTRAINT authenticatorapp_pk PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE "Identity"."AuthenticatorApp"
    OWNER to postgres;