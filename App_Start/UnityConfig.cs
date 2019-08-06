using CentralizedDataSystem.Repositories.Implements;
using CentralizedDataSystem.Repositories.Interfaces;
using CentralizedDataSystem.Services;
using CentralizedDataSystem.Services.Implements;
using CentralizedDataSystem.Services.Interfaces;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace CentralizedDataSystem {
    public static class UnityConfig {
        public static void RegisterComponents() {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            // Service
            container.RegisterType<IFormControlService, FormControlService>();
            container.RegisterType<IFormService, FormService>();
            container.RegisterType<IGroupService, GroupService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<ISubmissionService, SubmissionService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IWeatherService, WeatherService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IStatisticsService, StatisticsService>();
            container.RegisterType<ISendEmailService, SendEmailService>();

            // Repository
            container.RegisterType<IMongoRepository, MongoRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}