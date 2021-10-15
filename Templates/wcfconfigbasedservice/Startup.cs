/*
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace #NAMESPACEPLACEHOLDER#
{
   public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string pathToXml = #XMLPATH#;
            services.AddServiceModelServices();
            services.AddServiceModelConfigurationManagerFile(pathToXml);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel();
        }
    }
}
*/