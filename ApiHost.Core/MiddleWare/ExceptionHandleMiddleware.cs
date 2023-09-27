namespace ApiHost.Core.MiddleWare
{
    public class ExceptionHandleMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
                await next(context);
			}
			catch (Exception ex)
			{
                HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsync($"{context.Response.StatusCode} Internal Server Error from the ExceptionHandle middleware.{Environment.NewLine}{exception.ToString()}");
        }
    }
}
