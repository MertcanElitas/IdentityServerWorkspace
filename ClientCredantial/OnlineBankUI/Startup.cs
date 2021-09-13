using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankUI
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
            services.AddAuthentication(_ =>
            {
                _.DefaultScheme = "OnlineBankamatikCookie";
                _.DefaultChallengeScheme = "oidc";
            })
             .AddCookie("OnlineBankamatikCookie",options=> { options.AccessDeniedPath = "/Home/AccessDenied"; })
             .AddOpenIdConnect("oidc", _ =>
             {
                 _.SignInScheme = "OnlineBankamatikCookie";
                 _.Authority = "https://localhost:44310";
                 _.ClientId = "OnlineBankamatik";
                 _.ClientSecret = "onlinebankamatik";
                 _.ResponseType = "code id_token";
                 _.GetClaimsFromUserInfoEndpoint = true; //Normalde IdentityServerConstants.StandardScopes.Profile tanımlamasıyla bize gelecek olan 
                 //claimler bu property default da false olduğu için gelmez bunu true olarak işaretlersek profile altında bulunan bütün claimleri ekler ve gönderir.

                 _.SaveTokens = true; //Authentication propertylerden access-token değerinin kayıt edilmesi için true olarak set edilir.
                 _.Scope.Add("offline_access"); //Refresh tokenın kullanılabilmesi için gerekli olan scope "offline_access".

                 _.Scope.Add("XBank.Write");
                 _.Scope.Add("XBank.Read");
                 _.Scope.Add("PositionAndAuthority");
                 _.Scope.Add("Roles");

                 _.ClaimActions.MapUniqueJsonKey("position", "position");
                 _.ClaimActions.MapUniqueJsonKey("authority", "authority");
                 _.ClaimActions.MapUniqueJsonKey("role", "role");

                 _.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                 {
                     RoleClaimType = "role"
                 };
             });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
