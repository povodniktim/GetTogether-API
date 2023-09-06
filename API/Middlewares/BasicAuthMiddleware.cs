using System.Text;
using API.Helpers;

namespace API.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorised");
                return;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            var encodedCreds = header.Substring(6);
            var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
            string[] uidpwd = creds.Split(":");
            var uid = uidpwd[0];
            var pwd = uidpwd[1];

            if (uid != EnvHelper.GetEnv(EnvVariable.ApiUsername)
                || pwd != EnvHelper.GetEnv(EnvVariable.ApiPassword))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unathorized");
                return;
            }

            await _next(context);
        }
    }
}
