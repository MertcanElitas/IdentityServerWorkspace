using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankAuthUI
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
                        AllowedScopes = { "XBank.Write", "XBank.Read" },
                         RequireConsent=true
                    },
            new Client
                    {
                        ClientId = "YBank",
                        ClientName = "YBankName",
                        ClientSecrets = { new Secret("ybank".Sha256()) },
                        AllowedGrantTypes = { GrantType.ClientCredentials },
                        RequireConsent=true,
                        AllowedScopes = { "YBank.Write", "YBank.Read" }
                    },
            new Client
                {
                    ClientId = "OnlineBankamatik",
                    ClientName = "OnlineBankamatik",
                    ClientSecrets = { new Secret("onlinebankamatik".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId,
                                      IdentityServerConstants.StandardScopes.Profile,
                                      IdentityServerConstants.StandardScopes.OfflineAccess,
                                      "XBank.Write",
                                      "XBank.Read",
                                      "PositionAndAuthority",
                                      "Roles"},
                    RedirectUris = { "https://localhost:44358/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44358//signout-callback-oidc" },
                    RequirePkce = false,
                    RequireConsent=false,
                    AccessTokenLifetime = 2*60*60, //Access tokenın geçerlilik süresi.

                    //"IdentityServerConstants.StandardScopes.OfflineAccess" ilgili client için refresh tokena erişim yetkisi verilir.
                    //Refresh Token Configuration
                    AllowOfflineAccess=true,
                    RefreshTokenUsage=TokenUsage.OneTimeOnly, //Refresh tokenın kullanılabilirliği ayarlanır.Tek sefer mi kullanılacak yoksa tekrar tekrar kullanılabilirmi.
                    RefreshTokenExpiration=TokenExpiration.Absolute, //Refresh tokenın ömrünü belirler. Süre bitince direk expire mı olacak(Absolute) yoksa belli bir süre içinde kullanılırsa ömrü periyoduk uzayacakmı(Sliding).
                    AbsoluteRefreshTokenLifetime=2 * 60 * 60 + (10 * 60)
                }
        };
        }

        #endregion

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
                     new Claim("gender","1"),
                     new Claim("position" , "Test Kullanıcısı 1"),
                     new Claim("authority", "Test 1"),
                     new Claim("role", "admin")
                 }
             },
             new TestUser {
                 SubjectId = "test-user2",
                 Username = "test-user2",
                 Password = "12345",
                 Claims = {
                     new Claim("name","test user2"),
                     new Claim("website","https://wwww.testuser2.com"),
                     new Claim("gender","0"),
                     new Claim("position" , "Test Kullanıcısı 2"),
                     new Claim("authority", "Test 2"),
                     new Claim("role", "user")
                 }
             }
          };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
             {
                 new IdentityResources.OpenId(),
                 new IdentityResources.Profile(),
                 new IdentityResource()
                 {
                      Name="PositionAndAuthority",
                      DisplayName = "Position And Authority",
                      Description = "Kullanıcı pozisyonu ve yetkisi.",
                      UserClaims = { "position", "authority" }
                 },
                 new IdentityResource()
                 {
                      Name="Roles",
                      DisplayName = "Role",
                      Description = "Uygulama için claim bazlı yetki yönetimi",
                      UserClaims = { "role" }
                 }
             };
        }
    }
}
