CREATE VIEW [Portal].[vwAuthenticatorDevice]
AS
SELECT 
        ad.Id
    ,   ad.UserId
    ,   ad.Name
    ,   ad.WhenEnrolled
    ,   ad.WhenLastUsed
    ,   ad.CredentialId
    ,   ad.PublicKey
    ,   ad.Aaguid
    ,   ad.Counter
    ,   ad.CredType
    ,   ad.TenantId
FROM [Identity].AuthenticatorDevice ad
WHERE ad.WhenRevoked IS NULL;