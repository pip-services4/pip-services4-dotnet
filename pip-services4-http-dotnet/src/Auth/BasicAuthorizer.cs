using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Commons.Errors;
using PipServices4.Http.Controllers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PipServices4.Http.Auth
{
    public class BasicAuthorizer
    {
        public Func<HttpRequest, HttpResponse, ClaimsPrincipal, RouteData, Func<Task>, Task> Anybody()
        {
            return async (request, response, user, routeData, next) => { await next(); };
        }

        public Func<HttpRequest, HttpResponse, ClaimsPrincipal, RouteData, Func<Task>, Task> Signed()
        {
            return
                async (request, response, user, routeData, next) =>
                {
                    if (user == null || !user.Identity.IsAuthenticated)
                    {
                        await HttpResponseSender.SendErrorAsync(
                            response,
                            new UnauthorizedException(
                                null, "NOT_SIGNED",
                                "User must be signed in to perform this operation"
                            ).WithStatus(401)
                        );
                    }
                    else
                    {
                        await next();
                    }
                };
        }
    }
}