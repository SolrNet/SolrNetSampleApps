using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SampleSolrApp.Helpers;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Microsoft.DependencyInjection;

namespace SampleSolrApp
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
            services.AddControllersWithViews();
            services.AddSolrNet<Product>($"http://{Program.Solr.Value.Hostname}:{Program.Solr.Value.GetMappedPublicPort(8983)}/solr/techproducts");

            services.Replace(new ServiceDescriptor(
                typeof(ISolrInjectedConnection<Product>),
                s =>
                {
                    var logConn = new LoggingConnection(s.GetRequiredService<ISolrConnection>(), s.GetRequiredService<ILogger<LoggingConnection>>());
                    return new BasicInjectionConnection<Product>(logConn);
                },
                ServiceLifetime.Singleton
            ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();
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