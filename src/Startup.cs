using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sufficit.Blazor.UI.Material.Extensions;
using Sufficit.Telephony.EventsPanel;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Sufficit.Client;

namespace Sufficit.Telephony.BlazorPanel
{
    public class Startup
    {
        private IServerAddressesFeature? serverAddressesFeature;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Incluindo as configurações na injeção de dependencias
            services.TryAddSingleton<IConfiguration>(Configuration);

            // Configuração padrão para o gerenciamento de LOGS
            services.AddLogging();

            services.AddOptions();

            services.AddEventsPanel();
            services.AddSufficitAuthentication(); 
            services.AddSufficitEndPointsAPI();

            services.AddHttpClient("default");
            services.AddBlazorUIMaterial();
            
            services.AddHttpContextAccessor();
            services.Configure<HybridOptions>(Configuration);
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<GrpcExceptionInterceptor>();
                options.ResponseCompressionAlgorithm = "gzip";
                options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
                options.EnableDetailedErrors = true;
            });
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            /*
            services.AddCodeFirstGrpc(config =>
            {
                config.EnableDetailedErrors = true;
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });
            */
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseResponseCompression();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSufficitEndPointsAPI();


            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            app.Use((context, next) =>
            {
                var idCookiaName = "hubrid-instance-id";
                if (!context.Request.Cookies.Any(c => c.Key == idCookiaName))
                {
                    var idCookieOptions = new CookieOptions
                    {
                        Path = "/",
                        Secure = true,
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddYears(100),
                    };
                    context.Response.Cookies.Append(
                        key: idCookiaName,
                        value: Guid.NewGuid().ToString(),
                        options: idCookieOptions);
                }
                return next();
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<Services.WeatherForecastService>();
                //endpoints.MapGrpcService<Services.CounterService>();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
