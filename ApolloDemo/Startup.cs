using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApolloDemo
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
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/live");
                //endpoints.MapHealthChecks("/ready");
                //endpoints.MapHealthChecks("/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                //{
                //    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                //});
                endpoints.MapControllers();

                endpoints.MapDefaultControllerRoute();
            });
            //app.Run(context =>
            //{
            //    context.Response.StatusCode = 404;

            //    var key = context.Request.Query["key"];
            //    if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

            //    var value = context.RequestServices.GetRequiredService<IConfiguration>()[key];
            //    if (value != null) context.Response.StatusCode = 200;

            //    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

            //    return context.Response.WriteAsync(value ?? "undefined");
            //});
        }
    }

}
