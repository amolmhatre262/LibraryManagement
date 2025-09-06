using System.Net;

namespace Library_Management_Portal.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // continue pipeline
            }
            catch (Exception ex)
            {
                LogError(ex); // log to file
                await HandleExceptionAsync(context, ex);
            }
        }

        private void LogError(Exception ex)
        {
            string logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logDir);

            string logFile = Path.Combine(logDir, "errors.txt");

            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                writer.WriteLine("-----------");
                writer.WriteLine($"Date: {DateTime.Now}");
                writer.WriteLine($"Message: {ex.Message}");
                writer.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                    writer.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                writer.WriteLine("-----------\n");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/html";
            return context.Response.WriteAsync(
                "<h2>Oops! Something went wrong.</h2><p>Please try again later.</p>"
            );
        }
    }
}
