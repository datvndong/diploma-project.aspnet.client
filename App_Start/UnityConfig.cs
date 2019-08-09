using CentralizedDataSystem.Repositories.Implements;
using CentralizedDataSystem.Repositories.Interfaces;
using CentralizedDataSystem.Services.Implements;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Implements;
using CentralizedDataSystem.Utils.Interfaces;
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
            container.RegisterType<IBaseService, BaseService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IExportService, ExportService>();
            container.RegisterType<IFormControlService, FormControlService>();
            container.RegisterType<IFormService, FormService>();
            container.RegisterType<IGroupService, GroupService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<ISendEmailService, SendEmailService>();
            container.RegisterType<IStatisticsService, StatisticsService>();
            container.RegisterType<ISubmissionService, SubmissionService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IWeatherService, WeatherService>();

            // Repository
            container.RegisterType<IMongoRepository, MongoRepository>();

            // Util
            container.RegisterType<IHttpUtil, HttpUtil>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}