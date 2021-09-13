// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer
{
    public static class Config
    {
        #region " Scopes "

        //API'larda kullanılacak izinleri tanımlar.
        //APIlar üzerinden uygulanacak olan izinler.
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            var list = new List<ApiScope>()
            {
                new ApiScope("XBank.Write","XBank yazma izni"),
                new ApiScope("XBank.Read","XBank okuma izni"),
                new ApiScope("YBank.Write","YBank yazma izni"),
                new ApiScope("YBank.Read","YBank okuma izni")
            };

            return list;
        }

        #endregion
        #region " Resources "

        //API'lar tanımlanır.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            var list = new List<ApiResource>()
            {
                new ApiResource("XBank"){ApiSecrets={ new Secret("secretxbank".Sha256()) },Scopes={ "XBank.Write", "XBank.Read" }},
                new ApiResource("YBank"){ApiSecrets={ new Secret("secretybank".Sha256()) },Scopes={"YBank.Write", "YBank.Read" }}
            };

            return list;
        }

        #endregion
        #region " Clients "

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
        {
            new Client
                    {
                        ClientId = "XBank",
                        ClientName = "XBank",
                        ClientSecrets = { new Secret("xbank".Sha256()) },
                        AllowedGrantTypes = { GrantType.ClientCredentials },
                        AllowedScopes = { "XBank.Write", "XBank.Read" }
                    },
            new Client
                    {
                        ClientId = "YBank",
                        ClientName = "YBankName",
                        ClientSecrets = { new Secret("ybank".Sha256()) },
                        AllowedGrantTypes = { GrantType.ClientCredentials },
                        AllowedScopes = { "YBank.Write", "YBank.Read" }
                    }
        };
        }

        #endregion
        #region " Users "

        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
        new TestUser {
            SubjectId = "test-user1",
            Username = "test-user1",
            Password = "12345",
            Claims = {
                new Claim("name","test user1"),
                new Claim("website","https://wwww.testuser1.com"),
                new Claim("gender","1")
            }
        },
        new TestUser {
            SubjectId = "test-user2",
            Username = "test-user2",
            Password = "12345",
            Claims = {
                new Claim("name","test user2"),
                new Claim("website","https://wwww.testuser2.com"),
                new Claim("gender","0")
            }
        }
    };
        }

        #endregion
        #region " IdentityResources "

        //Açıklama information.txt
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //Üretilecek token içerisinde kesinlikle bir kullanıcı id/user id/subject id olmalıdır.
                                                //OpenId kullanıcı id değerini ifade eder. Token’da ‘subid’ olarak tutulacaktır

                new IdentityResources.Profile() //Kullanıcı profil bilgilerini ve biryandan da kullanıcı için var olan tüm claim’leri barındırır.
            };
        }

        #endregion
    }
}