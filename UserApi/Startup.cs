using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserApi
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
            //userApi 支持jwt reference
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "userApi";
                    options.ApiSecret = "userapiSecret";
                    options.RequireHttpsMetadata = false;

                    //支持两种方式
                    //reference 验证路径 http://localhost:5000/connect/introspect
                    //reference 撤销路径 http://localhost:5000/connect/revocation
                    options.SupportedTokens = SupportedTokens.Both;

                    //配置缓存
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            //services.AddSingleton<IConsulClient>(cfg =>
            //{
            //    return new ConsulClient(x =>
            //    {
            //        x.Address = new Uri("http://127.0.0.1:8500");
            //    });
            //});

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
        public void Configure(IApplicationBuilder app,IApplicationLifetime lifetime,IConsulClient consulClient)
        {
            //var consulId = "userApiId" + new Guid();
            //lifetime.ApplicationStarted.Register(() =>
            //{
            //    //var serviceRegister = new AgentServiceRegistration
            //    //{
            //    //    ID=consulServiceId,
            //    //    Name="userApiName",
            //    //    Port=5002,
            //    //    Address="127.0.0.1",
            //    //    Check=new AgentServiceCheck
            //    //    {
            //    //        Interval=TimeSpan.FromSeconds(30),
            //    //        DeregisterCriticalServiceAfter=TimeSpan.FromSeconds(30),
            //    //        Timeout=TimeSpan.FromSeconds(30),
            //    //        HTTP="http://localhost:5002/api/healthcheck"
            //    //    }
            //    //};
            //    //consulClient.Agent.ServiceRegister(serviceRegister);

            //    var agentService = new AgentServiceRegistration
            //    {
            //        ID = consulId,
            //        Address = "127.0.0.1",
            //        Name = "userApiName",
            //        Port = 5002,
            //        Check = new AgentServiceCheck
            //        {
            //            Timeout = TimeSpan.FromSeconds(5),
            //            Interval = TimeSpan.FromSeconds(30),
            //            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
            //            HTTP = "http://localhost:5002/api/healthcheck"
            //        }
            //    };
            //    consulClient.Agent.ServiceRegister(agentService);
            //});

            //lifetime.ApplicationStopped.Register(() =>
            //{
            //    consulClient.Agent.ServiceDeregister(consulId).ConfigureAwait(false);
            //});

            var consulId = "usersApiId" + new Guid();

            lifetime.ApplicationStarted.Register(() =>
            {
                var agentService = new AgentServiceRegistration
                {
                    ID = consulId,
                    Address = "127.0.0.1",
                    Name = "usersApiName",
                    Port = 5002,
                    Check = new AgentServiceCheck
                    {
                        Timeout = TimeSpan.FromSeconds(5),
                        Interval = TimeSpan.FromSeconds(30),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                        HTTP = "http://localhost:5002/api/healthcheck"
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
