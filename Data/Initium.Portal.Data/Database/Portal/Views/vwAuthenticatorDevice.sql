CREATE OR REPLACE VIEW "Portal"."vwAuthenticatorDevice"
AS
SELECT ad."Id",
       ad."UserId",
       ad."Name",
       ad."WhenEnrolled",
       ad."WhenLastUsed",
       ad."CredentialId",
       ad."PublicKey",
       ad."Aaguid",
       ad."Counter",
       ad."CredType"
FROM "Identity"."AuthenticatorDevice" ad
WHERE ad."WhenRevoked" IS NULL;

ALTER TABLE "Portal"."vwAuthenticatorDevice"
    OWNER TO postgres;