using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutlookAPI
{
    public class Startup
    {
        private readonly string  _appClientID = "be27180b-edfc-4c41-a7a3-c53ce1d0ba9d";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Outlook API",
                    Version = "v1",
                    Description = "An .NET 5.0 Core API for managing Outlook Events",
                });

                // Use method name as operationId
                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out System.Reflection.MethodInfo methodInfo) ? methodInfo.Name : null;
                });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize"), 
                            TokenUrl = new Uri($"https://login.microsoftonline.com/common/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "https://graph.microsoft.com/Calendars.ReadWrite", "Read my Calendars" },

                                //{ "api://81f0b980-a06b-412e-895f-1446483bdb9f/Manage.MyOutlook", "Manage my outlook events" },
                                //{ "api://81f0b980-a06b-412e-895f-1446483bdb9f/offline_access", "Manage my outlook events offline" },
                                //{ "api://81f0b980-a06b-412e-895f-1446483bdb9f/User.Read", "Read my user information" }
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference{
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            In = ParameterLocation.Header
                        },
                        new[] { "readAccess", "writeAccess" }
                    }
                });

                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename);
                c.IncludeXmlComments(filePath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Outlook API v1");

                    c.OAuthClientId(_appClientID);
                    c.OAuthRealm("client-realm");
                    c.OAuthAppName("Coda-Outlook");
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });
            }

            app.UseHttpsRedirection();

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
