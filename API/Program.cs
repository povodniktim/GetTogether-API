using API.Models;
using API.Middlewares;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<GetTogetherContext>();

            var config =
                new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<BasicAuthMiddleware>();
            app.UseMiddleware<TokenValidationMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}