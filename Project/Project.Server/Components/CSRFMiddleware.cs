using Microsoft.AspNetCore.Antiforgery;

namespace Project.Server.Components
{
    public class CSRFMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public CSRFMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext httpContext){
            // Skip CSRF for GET requests, etc.
            if (HttpMethods.IsGet(httpContext.Request.Method) ||
                    HttpMethods.IsOptions(httpContext.Request.Method) ||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/login") ||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/register") ||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/check/password/email")||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/send/reset-password") ||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/validate/reset-password-token")||
                    httpContext.Request.Path.StartsWithSegments("/api/authentication/reset-password"))
            {
                await _next(httpContext);
                return;
            }

            var csrfTokenFromHeader = httpContext.Request.Headers["x-xrsf-token"].ToString();
            var csrfTokenFromCookie = httpContext.Request.Cookies["XSRF-TOKEN"];

            Console.WriteLine("CSRF Token from Header: " + csrfTokenFromHeader);
            Console.WriteLine("CSRF Token from Cookie: " + csrfTokenFromCookie);

            if (string.IsNullOrEmpty(csrfTokenFromHeader) || csrfTokenFromHeader != csrfTokenFromCookie)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsync("Invalid CSRF token");
                return;
            }

            await _next(httpContext);
        }
     }   
}
