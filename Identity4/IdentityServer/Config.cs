// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
                                           name: "custom.profile",
                                           displayName: "Custom profile",
                                           claimTypes: new[] { "name", "email", "status" });
            return new List<IdentityResource>{
                        new IdentityResources.OpenId(),
                        new IdentityResources.Email(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Phone(),
                        new IdentityResources.Address(),
                        customProfile
            };
        }
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource
                {
                        Name = "api2",

                        // secret for using introspection endpoint
                        ApiSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        // include the following using claims in access token (in addition to subject id)
                        UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email },

                        // this API defines two scopes
                        Scopes =
                        {
                            new Scope()
                            {
                                Name = "api2.full_access",
                                DisplayName = "Full access to API 2",
                            },
                            new Scope
                            {
                                Name = "api2.read_only",
                                DisplayName = "Read only access to API 2"
                            }
                    }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",//demo

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                //Server到Server的客户端 or app to app
                new Client
                {
                    ClientId = "service.client",    
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1", "api2.read_only" }
                },
                //JavaScript客户端（例如SPA）进行用户认证和授权访问和API
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "http://localhost:7017/index.html" },
                    PostLogoutRedirectUris = { "http://localhost:7017/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:7017" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,

                        "api1", "api2.read_only"
                    }
                },
                //定义服务器端Web应用程序（例如MVC）以进行使用验证和授权的API访问
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    RedirectUris =           { "http://localhost:21402/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:21402/" },
                    FrontChannelLogoutUri =  "http://localhost:21402/signout-oidc",

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,

                        "api1", "api2.read_only"
                    },
                }
            };
    }
}