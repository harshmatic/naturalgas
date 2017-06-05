using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ESPL.NG.Services;
using Microsoft.EntityFrameworkCore;
using ESPL.NG.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Diagnostics;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;
using System.Linq;
using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ESPL.NG.Entities;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ESPL.NG.Models;
// using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ESPL.NG.Helpers.Core;
using System;
using System.Reflection;
using MySQL.Data.Entity.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using System.IdentityModel.Tokens.Jwt;
using AspNet.Security.OpenIdConnect.Server;

namespace ESPL.NG
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration["connectionStrings:naturalGasDBConnectionString"];
            services.AddDbContext<Entities.ApplicationContext>(o => o.UseMySQL(connectionString));
            services.AddTransient<IdentityInitializer>();

            services.AddAuthentication();

            services.AddCors();
            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                // setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());

                var xmlDataContractSerializerInputFormatter =
                new XmlDataContractSerializerInputFormatter();
                xmlDataContractSerializerInputFormatter.SupportedMediaTypes
                    .Add("application/vnd.marvin.authorwithdateofdeath.full+xml");
                setupAction.InputFormatters.Add(xmlDataContractSerializerInputFormatter);

                var jsonInputFormatter = setupAction.InputFormatters
                .OfType<JsonInputFormatter>().FirstOrDefault();

                if (jsonInputFormatter != null)
                {
                    jsonInputFormatter.SupportedMediaTypes
                    .Add("application/vnd.marvin.author.full+json");
                    jsonInputFormatter.SupportedMediaTypes
                    .Add("application/vnd.marvin.authorwithdateofdeath.full+json");
                }

                var jsonOutputFormatter = setupAction.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }

            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling =
                ReferenceLoopHandling.Ignore;
                // options.SerializerSettings.PreserveReferencesHandling =
                // PreserveReferencesHandling.Objects;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsSuperAdmin", policy => policy.RequireClaim("IsSuperAdmin"));
            });

            services.AddNodeServices();

            // register the repository
            services.AddScoped<IAppRepository, AppRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddHttpCacheHeaders(
                (expirationModelOptions)
                =>
                {
                    expirationModelOptions.MaxAge = 600;
                },
                (validationModelOptions)
                =>
                {
                    validationModelOptions.AddMustRevalidate = true;
                });

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 1000,
                        Period = "5m"
                    },
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 200,
                        Period = "10s"
                    }
                };
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, Entities.ApplicationContext naturalGasContext,
            IdentityInitializer identitySeeder)
        {
            loggerFactory.AddConsole();

            loggerFactory.AddDebug(LogLevel.Information);

            //  loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());

            loggerFactory.AddNLog();

            app.UseCors(cfg =>
            {
                cfg.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

                    });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ESPL.NG.Entities.AppUser, ESPL.NG.Models.Core.AppUserDto>();
                cfg.CreateMap<ESPL.NG.Models.Core.AppUserForCreationDto, ESPL.NG.Entities.AppUser>();
                cfg.CreateMap<ESPL.NG.Entities.Customer, ESPL.NG.Models.CustomerDto>();
                cfg.CreateMap<ESPL.NG.Models.CustomerForCreationDto, ESPL.NG.Entities.Customer>();
                cfg.CreateMap<ESPL.NG.Entities.Customer, ESPL.NG.Models.CustomerForCreationDto>();
                cfg.CreateMap<ESPL.NG.Models.CustomerForUpdationDto, ESPL.NG.Entities.Customer>();
                cfg.CreateMap<ESPL.NG.Entities.Customer, ESPL.NG.Models.CustomerForUpdationDto>();
            });

            // identitySeeder.Seed().Wait();
            naturalGasContext.EnsureSeedDataForContext();
            app.UseIpRateLimiting();
            app.UseHttpCacheHeaders();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            // app.UseIdentity();

            //app.UseOAuthValidation();
            app.UseOpenIdConnectServer(options =>
            {
                // Enable the token endpoint.
                options.TokenEndpointPath = "/connect/token";
                options.AllowInsecureHttp = true;
                // Implement OnValidateTokenRequest to support flows using the token endpoint.
                options.Provider.OnValidateTokenRequest = context =>
                {
                    // Reject token requests that don't use grant_type=password or grant_type=refresh_token.
                    if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
                    {
                        context.Reject(
                            error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                            description: "Only grant_type=password and refresh_token " +
                                         "requests are accepted by this server.");

                        return Task.FromResult(0);
                    }

                    // Note: you can skip the request validation when the client_id
                    // parameter is missing to support unauthenticated token requests.
                    // if (string.IsNullOrEmpty(context.ClientId))
                    // {
                    //     context.Skip();
                    // 
                    //     return Task.FromResult(0);
                    // }

                    // Note: to mitigate brute force attacks, you SHOULD strongly consider applying
                    // a key derivation function like PBKDF2 to slow down the secret validation process.
                    // You SHOULD also consider using a time-constant comparer to prevent timing attacks.
                    if (string.Equals(context.ClientId, "client_id", StringComparison.Ordinal) &&
                        string.Equals(context.ClientSecret, "client_secret", StringComparison.Ordinal))
                    {
                        context.Validate();
                    }

                    // Note: if Validate() is not explicitly called,
                    // the request is automatically rejected.
                    return Task.FromResult(0);
                };

                // Implement OnHandleTokenRequest to support token requests.
                options.Provider.OnHandleTokenRequest = context =>
                {
                    // Only handle grant_type=password token requests and let the
                    // OpenID Connect server middleware handle the other grant types.
                    if (context.Request.IsPasswordGrantType())
                    {
                        // Implement context.Request.Username/context.Request.Password validation here.
                        // Note: you can call context Reject() to indicate that authentication failed.
                        // Using password derivation and time-constant comparer is STRONGLY recommended.
                        if (!string.Equals(context.Request.Username, "Bob", StringComparison.Ordinal) ||
                            !string.Equals(context.Request.Password, "P@ssw0rd", StringComparison.Ordinal))
                        {
                            context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidGrant,
                                description: "Invalid user credentials.");

                            return Task.FromResult(0);
                        }

                        var identity = new ClaimsIdentity(context.Options.AuthenticationScheme,
                            OpenIdConnectConstants.Claims.Name,
                            OpenIdConnectConstants.Claims.Role);

                        // Add the mandatory subject/user identifier claim.
                        identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[unique id]");

                        // By default, claims are not serialized in the access/identity tokens.
                        // Use the overload taking a "destinations" parameter to make sure
                        // your claims are correctly inserted in the appropriate tokens.
                        identity.AddClaim("urn:customclaim", "value",
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken);

                        var ticket = new AuthenticationTicket(
                            new ClaimsPrincipal(identity),
                            new AuthenticationProperties(),
                            context.Options.AuthenticationScheme);

                        // Call SetScopes with the list of scopes you want to grant
                        // (specify offline_access to issue a refresh token).
                        ticket.SetScopes(
                            OpenIdConnectConstants.Scopes.Profile,
                            OpenIdConnectConstants.Scopes.OfflineAccess);

                        context.Validate(ticket);
                    }

                    return Task.FromResult(0);
                };
            });

            // app.UseJwtBearerAuthentication(new JwtBearerOptions()
            // {
            //     AutomaticAuthenticate = true,
            //     AutomaticChallenge = true,
            //     TokenValidationParameters = new TokenValidationParameters()
            //     {
            //         ValidIssuer = Configuration["Tokens:Issuer"],
            //         ValidAudience = Configuration["Tokens:Audience"],
            //         ValidateIssuerSigningKey = true,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
            //         ValidateLifetime = true
            //     },
            // });

            app.UseMvc();
        }

    }
}
