using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MassTransit;

namespace autocomplete_example
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IBusControl BusControl { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BusControl =  Bus.Factory.CreateUsingRabbitMq(x => x.Host(new Uri("rabbitmq://localhost"), h =>
            {
                h.Username("guest");
                h.Password("guest");
            }));

            BusControl.Start();
        }

        protected void Application_End()
        {
            BusControl.Stop();
        }
    }
}
