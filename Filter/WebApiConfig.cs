using System.Web.Http;
using WebApiAuthenticate.Filters;

namespace WebApiAuthenticate
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services        

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Endereco",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new BasicAuthenticationAttribute());
        }
    }
}