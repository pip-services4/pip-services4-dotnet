using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using System.Collections.Generic;

namespace PipServices4.Swagger.Services
{
    public class SwaggerController : IConfigurable, IReferenceable, ISwaggerController, IInitializable
    {
        private static readonly ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "dependencies.endpoint", "*:endpoint:http:*:1.0"
        );

        /// <summary>
        /// The HTTP endpoint that exposes this service.
        /// </summary>
        protected HttpEndpoint _endpoint;
        /// <summary>
        /// The dependency resolver.
        /// </summary>
        protected DependencyResolver _dependencyResolver = new DependencyResolver(_defaultConfig);

        /// <summary>
        /// Routes with swagger doc
        /// </summary>
        private Dictionary<string, string> _routes = new Dictionary<string, string>();



        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            config = config.SetDefaults(_defaultConfig);
            _dependencyResolver.Configure(config);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public virtual void SetReferences(IReferences references)
        {
            _dependencyResolver.SetReferences(references);

            // Get endpoint
            _endpoint = _dependencyResolver.GetOneOptional("endpoint") as HttpEndpoint;

            // Add initialization callback to the endpoint
            if (_endpoint != null)
                _endpoint.Initialize(this);
        }

        private string AppendBaseRoute(string baseRoute, string route)
        {
            if (!string.IsNullOrEmpty(baseRoute))
            {
                if (string.IsNullOrEmpty(route))
                    route = "/";
                if (route[0] != '/')
                    route = "/" + route;
                if (baseRoute[0] != '/') baseRoute = '/' + baseRoute;
                route = baseRoute + route;
            }

            return route;
        }

        public void RegisterOpenApiSpec(string baseRoute, string swaggerRoute)
        {
            var route = AppendBaseRoute(baseRoute, swaggerRoute);
            _routes.Add(route, baseRoute);
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void ConfigureApplication(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder
                .UseSwaggerUI(c =>
                {
                    foreach (var route in _routes)
                    {
                        c.SwaggerEndpoint(route.Key, route.Value);
                    }
                });
        }
    }
}
