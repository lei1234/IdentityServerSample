using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter
{
    public class MyConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("orderApi"),
                //如果资源api支持reference需要单独配置secret,userApi支持jwt reference两种
                new ApiResource("userApi",new List<string>{ "userName","phone"}){ApiSecrets={new Secret("userapiSecret".Sha256()) } }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="jwt",
                    ClientSecrets={new Secret("jwt-secret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    //配置客户端的token验证方式
                    //eyJhbGciOiJSUzI1NiIsImtpZCI6IjViY2UyZjljNTBhMWRiZTc4MGE2YTM2NTlmZjdiNDY2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NTc5MjI5MTcsImV4cCI6MTU1NzkyNjUxNywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJvcmRlckFwaSJdLCJjbGllbnRfaWQiOiJqd3QiLCJzY29wZSI6WyJvcmRlckFwaSJdfQ.ECYvZn7pMUxLOcZLgA1LX1oGu8YWspVhmOubBVlWG_tCtP7Wcpr5gHdM1hY5ZA62rKfUP8QgflDz3vTrfhhMN7In8Y3ySaFruCPSDtXdp9t2upbB5OFVkdTT_1M7ubSTHcPDGjH5LqcxxrU_0pViXqb00w0a_vRamr4CLX6be0p7Jd7b5JOoS0WQNYPs2jiKMSO6GJXonrAPhdW5MlZmX7E_sfoYjjWi76XanGz5pyS_s02zCMaOOzJXDqVp2c1WS4Q6o3vGStcRqodRWsrXBSd0bJMAejDaVGXTKT514XonyTKzk-sBC4m5uEpAI72QVr3_O1NQdJJdUrzeJ78GRA
                    AccessTokenType=AccessTokenType.Jwt,
                    AllowedScopes={ "orderApi" ,"userApi"}
               },
                new Client
                {
                    ClientId="reference",
                    ClientSecrets={new Secret("reference-secret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    //571542530ec862317aa995b1565bc1a080984970e6fcf81f8ae7df2b8a1a338e
                    AccessTokenType=AccessTokenType.Reference,
                    //由于orderApi只支持jwt，而客户端是reference，所以访问不了orderApi
                    AllowedScopes={ "orderApi" ,"userApi"}
                },
                //如果客户端为password模式，api资源申明返回的claim（'userName','phone'），
                //可以通过定义profileservice将claim返回给客户端
                new Client
                {
                    ClientId="pwd-client",
                    ClientSecrets={new Secret("pwd-secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={"orderApi","userApi"}

                },
                //配置MVC client
                new Client
                {
                    ClientId="mvc-id",
                    ClientSecrets={new Secret("mvc-secret".Sha256()) },
                    //当客户端ResponseType="id_token" 返回 id_token 
                    //code id_token
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    RedirectUris={"http://localhost:58319/signin-oidc"},
                    PostLogoutRedirectUris={"http://localhost:58319/signout-callback-oidc" },
                    //讲profileservice信息通过idtoken返回给客户端
                    AlwaysIncludeUserClaimsInIdToken=true,
                    //配置讲refrece token返回给客户端,
                    AllowOfflineAccess=true,
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        //配置refrece token给客户端
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "userApi"
                    },
                    AccessTokenLifetime = 1800,//设置AccessToken过期时间
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    
                    //RefreshTokenExpiration = TokenExpiration.Absolute,//刷新令牌将在固定时间点到期
                    AbsoluteRefreshTokenLifetime = 2592000,//RefreshToken的最长生命周期,默认30天
                    RefreshTokenExpiration = TokenExpiration.Sliding,//刷新令牌时，将刷新RefreshToken的生命周期。RefreshToken的总生命周期不会超过AbsoluteRefreshTokenLifetime。
                    SlidingRefreshTokenLifetime = 3600,//
                    

                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<TestUser> GetTestUser()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="adamson",
                    Username="adamson",
                    Password="pwd123"
                }
            };
        }
    }
}
