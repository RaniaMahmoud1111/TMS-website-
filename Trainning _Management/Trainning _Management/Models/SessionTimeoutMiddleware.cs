namespace Trainning__Management.Models
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the session has expired
            if (!context.Session.TryGetValue("UserId", out _))
            {
                var path = context.Request.Path.ToString();
                // Ignore the login path to avoid infinite redirect loop
                if (!path.StartsWith("/Login") && !path.StartsWith("/IRegister") && !path.StartsWith("/TRegister") && !path.StartsWith("/Home/Index"))
                {
                    context.Response.Redirect("/Login/Login");
                    return;
                }
            }

            await _next(context);
        }
    }

}
