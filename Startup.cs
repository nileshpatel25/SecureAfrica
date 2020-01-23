using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureAfrica.HubConfig;
using SecureAfrica.Models;

namespace SecureAfrica
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));
            services.AddSignalR();
          //  services.AddCors();
            //services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddXmlSerializerFormatters();
            services.AddMvc();
            services.AddDbContextPool<AppDbContex>(options => options.UseSqlServer(_config.GetConnectionString("SecureAfricaDbConection")));
            services.AddDbContext<AppDbContex>(options => options
                                      .UseSqlServer(_config.GetConnectionString("SecureAfricaDbConection")), ServiceLifetime.Scoped);
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContex>()
                    .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
                     {
                         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                         options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                     })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidAudience = "http://saveafrica.com",
                            ValidIssuer = "http://saveafrica.com",
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey"))
                        };
                    });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseCors(x => x
            //   .AllowAnyOrigin()
            //   .AllowAnyMethod()
            //   .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotifyHub>("/notify");
                routes.MapHub<ChartHub>("/chart");

            });
          
            app.UseMvc();
        }
    }
}
