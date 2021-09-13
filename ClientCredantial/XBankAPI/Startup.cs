using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XBankAPI
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //Token'ı yayınlayan Auth Server adresi bildiriliyor. Yani yetkiyi dağıtan mekanizmanın adresi bildirilerek ilgili API ile ilişkilendiriliyor.
                    options.Authority = "https://localhost:44310";
                    //Auth Server uygulamasındaki 'XBank' isimli resource ile bu API ilişkilendiriliyor.
                    options.Audience = "XBank";
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("ReadXBank", policy => policy.RequireClaim("scope", "XBank.Read"));
                configure.AddPolicy("WriteXBank", policy => policy.RequireClaim("scope", "XBank.Write"));
                configure.AddPolicy("ReadWriteXBank", policy => policy.RequireClaim("scope", "XBank.Write", "XBank.Read"));
                configure.AddPolicy("AllXBank", policy => policy.RequireClaim("scope", "XBank.Admin"));
                configure.AddPolicy("ReadYBank", policy => policy.RequireClaim("scope", "YBank.Read"));
                configure.AddPolicy("WriteYBank", policy => policy.RequireClaim("scope", "YBank.Write"));
                configure.AddPolicy("ReadWriteYBank", policy => policy.RequireClaim("scope", "YBank.Write", "YBank.Read"));
                configure.AddPolicy("AllYBank", policy => policy.RequireClaim("scope", "YBank.Admin"));
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
