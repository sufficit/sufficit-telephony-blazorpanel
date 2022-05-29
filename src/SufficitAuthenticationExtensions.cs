﻿using Sufficit.Identity.Configuration;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Sufficit.Telephony.BlazorPanel
{
    public static class SufficitAuthenticationExtensions
    {
        public static IServiceCollection AddSufficitAuthentication(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<OpenIDOptions>(options => configuration.GetSection(OpenIDOptions.SECTIONNAME));
            services.Configure<Identity.Configuration.CookieOptions>(options => configuration.GetSection(Identity.Configuration.CookieOptions.SECTIONNAME));

            // Capturando para uso local
            var oidOptions = configuration.GetSection(OpenIDOptions.SECTIONNAME).Get<OpenIDOptions>();

            // Capturando para uso local
            var cookieOptions = configuration.GetSection(Identity.Configuration.CookieOptions.SECTIONNAME).Get<Identity.Configuration.CookieOptions>();

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme =
                     CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme =
                   OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Domain = cookieOptions.Domain;
                options.Cookie.Name = cookieOptions.Name;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = oidOptions.Authority;
                options.ClientId = oidOptions.ClientId; 
                options.ClientSecret = oidOptions.ClientSecret;
                options.ResponseType = oidOptions.ResponseType;
                options.SaveTokens = oidOptions.SaveTokens;
                options.GetClaimsFromUserInfoEndpoint = oidOptions.GetClaimsFromUserInfoEndpoint;

                if (oidOptions.Scopes != null)
                {
                    options.Scope.Clear();
                    foreach (var scope in oidOptions.Scopes)
                        options.Scope.Add(scope);
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}