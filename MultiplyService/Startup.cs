using Common.Models;
using Common.Services;
using ExternalService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ExternalService
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IGeneratorService, Services.GeneratorService>();
            services.AddSingleton(Configuration);
            services.AddScoped<IMultiplierService, MultiplierService>();
            services.AddScoped<IServerClient<Multiplication>, ServerClient<Multiplication>>();
            services.AddScoped<IServerClient<BatchNumbersGenerated>, ServerClient<BatchNumbersGenerated>>();
            services.AddSingleton<ICancellationService, CancellationService>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
