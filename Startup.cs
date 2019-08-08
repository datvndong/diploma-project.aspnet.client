using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Utils.Implements;
using CentralizedDataSystem.Utils.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(CentralizedDataSystem.Startup))]

namespace CentralizedDataSystem {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddHttpClient<IHttpUtil, HttpUtil>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(Configs.HTTP_LIFE_TIME));
        }
    }
}
