namespace TailorSoftAPI.Extensions.Middleware
{
    public static class MiddlewareConfiguration
    {
        public static WebApplication UseMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");
            app.UseExceptionHandler();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

    }
}
