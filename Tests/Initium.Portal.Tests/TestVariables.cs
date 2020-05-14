// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Tests
{
    public static class TestVariables
    {
        public static readonly Guid AuthenticatorDeviceId = Guid.Parse("b10998c4-0e13-4f9a-9bc8-ce037e74758d");

        public static readonly Guid AuthenticatorDeviceAaguid = Guid.Parse("1e2441fa-be96-4d7d-a38c-1a20b7e80714");

        public static readonly byte[] AuthenticatorDevicePublicKey =
            Guid.Parse("c1fcc40f-ef90-442f-aa48-a91b87b2635b").ToByteArray();

        public static readonly byte[] AuthenticatorDeviceCredentialId =
            Guid.Parse("3dd7b064-cfe2-4dd3-b6c0-5580e6fce755").ToByteArray();

        public static readonly Guid RoleId = Guid.Parse("D1DB0F93-A737-4E3F-BB6A-EEE5A402E126");

        public static readonly Guid ResourceId = Guid.Parse("DE7A9184-FC70-4578-A645-1787886829D3");

        public static readonly Guid AuthenticationHistoryId = Guid.Parse("DE907A59-C87C-471B-9602-C9A9E2F0CC19");

        public static readonly DateTime Now = DateTime.Now;

        public static readonly Guid AuthenticatorAppId = Guid.Parse("1E25C5AC-7099-4B5D-9B18-691804D43E61");

        public static readonly Guid UserId = Guid.Parse("AFD3A44B-C043-494F-9B95-760FBEB26E4D");

        public static readonly Guid SecurityTokenMappingId = Guid.Parse("6F3645C5-AF9A-41B7-96E1-B81514208FA8");

        public static readonly Guid PasswordHistoryId = Guid.Parse("91354C95-F1F3-42C1-961F-0339D25A8855");

        public static readonly string OverridenPassword = "$2a$11$JkbCP/ludbTfmRmdRXVAN.1RNFoQuDGrdjCQzJOJizNMSoOWtYKxe";

        public static readonly Guid AuthenticatedUserId = Guid.Parse("A0E25FD5-53A0-4B30-BB8B-952573382199");
    }
}