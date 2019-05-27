using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderApi
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
            //orderApi采用的是JWT token
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "orderApi";
                });

            services.AddSingleton<IConsulClient>(cfg =>
            {
                return new ConsulClient((x) =>
                {
                    x.Address = new Uri("http://127.0.0.1:8500");
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IConsulClient consulClient,IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var consulId = "orderApiId" + new Guid();

            lifetime.ApplicationStarted.Register(() =>
            {
                var agentService = new AgentServiceRegistration
                {
                    ID=consulId,
                    Address="127.0.0.1",
                    Name="orderApiName",
                    Port=5001,
                    Check=new AgentServiceCheck
                    {
                        Timeout=TimeSpan.FromSeconds(5),
                        Interval=TimeSpan.FromSeconds(30),
                        DeregisterCriticalServiceAfter=TimeSpan.FromSeconds(30),
                        HTTP="http://localhost:5001/api/healthcheck"
                    }
                };
                consulClient.Agent.ServiceRegister(agentService);
            });

            lifetime.ApplicationStopped.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(consulId).ConfigureAwait(false);
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
