namespace ApiHost.Core.MiddleWare
{
    public class ResultMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return next(context);
        }
    }
}
