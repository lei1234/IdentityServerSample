using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MvcClientNormal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignInScheme = "cookie";
                options.DefaultScheme = "cookie";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc",options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:5000";
                options.ClientId = "mvc-id";
                options.ClientSecret = "mvc-secret";
                options.SignInScheme = "cookie";

                options.ClaimActions.Remove("amr");
                options.ClaimActions.Remove("exp");

                options.ClaimActions.MapUniqueJsonKey("userName", "userName");
                options.ClaimActions.MapUniqueJsonKey("phone", "phone");

                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;

                //如果客户端ResponseType="id_token"，服务端为implicat,
                //服务端为implicat,客户端Response="id_token token"或"code id_token";都会报错

                options.Scope.Add("offline_access");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.ResponseType = "id_token code";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
