using Sufficit.Identity.Configuration;
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
            var provider = services.BuildServiceProvider(false);
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<OpenIDOptions>(options => configuration.GetSection(OpenIDOptions.SECTIONNAME));
            services.Configure<Identity.Configuration.CookieOptions>(options => configuration.GetSection(Identity.Configuration.CookieOptions.SECTIONNAME));

            var authBuilder = services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = OpenIdConnectDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });

            // Capturando para uso local
            var cookieOptions = configuration.GetSection(Identity.Configuration.CookieOptions.SECTIONNAME).Get<Identity.Configuration.CookieOptions>();
            if (cookieOptions == null) {
                cookieOptions = new Identity.Configuration.CookieOptions();
                cookieOptions.Domain = "localhost";
                cookieOptions.Name = "Identity.BlazorPanel";
            }

            authBuilder.AddCookie(options =>
            {
                options.Cookie.Domain = cookieOptions.Domain;
                options.Cookie.Name = cookieOptions.Name;
            });

            // Capturando para uso local
            var oidOptions = configuration.GetSection(OpenIDOptions.SECTIONNAME).Get<OpenIDOptions>();

            if (oidOptions != null)
            {
                authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = oidOptions.Authority;
                    options.ClientId = oidOptions.ClientId;
                    options.ClientSecret = oidOptions.ClientSecret;
                    options.SaveTokens = oidOptions.SaveTokens;
                    options.GetClaimsFromUserInfoEndpoint = oidOptions.GetClaimsFromUserInfoEndpoint;

                    if (oidOptions.ResponseType != null)
                        options.ResponseType = oidOptions.ResponseType;

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
            }

            return services;
        }
    }
}
