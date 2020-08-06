CREATE TABLE "Identity"."AuthenticatorDevice"
(
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" character varying(200) COLLATE pg_catalog."default" NOT NULL,
    "WhenEnrolled" timestamp(6) without time zone NOT NULL,
    "WhenLastUsed" timestamp(6) without time zone,
    "WhenRevoked" timestamp(6) without time zone,
    "CredentialId" bytea NOT NULL,
    "PublicKey" bytea NOT NULL,
    "Aaguid" uuid NOT NULL,
    "Counter" integer NOT NULL,
    "CredType" character varying(200) COLLATE pg_catalog."default",
    CONSTRAINT authenticatordevice_pk PRIMARY KEY ("Id")
)

    TABLESPACE pg_default;

ALTER TABLE "Identity"."AuthenticatorDevice"
    OWNER to postgres;