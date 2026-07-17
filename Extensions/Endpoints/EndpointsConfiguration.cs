using Scalar.AspNetCore;

namespace TailorSoftAPI.Extensions.Endpoints
{
    public static class EndpointsConfiguration
    {
        public static WebApplication UseEndpoints(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                ConfigureScalarUI(app);
            }

            app.MapControllers();
            return app;
        }

        private static void ConfigureScalarUI(WebApplication app)
        {
            app.MapScalarApiReference(options =>
            {
                options.Title = "TailorSoft API";
                options.DarkMode = true;
                options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.AddPreferredSecuritySchemes("Bearer")
                    .AddHttpAuthentication("Bearer", auth =>
                    {
                        auth.Token = "jwt_token_here";
                    })
                    .EnablePersistentAuthentication();
            });
        }
    }
}
