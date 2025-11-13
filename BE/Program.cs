
using BE.Constants;
using BE.Data;
using BE.Repositories;
using BE.Services;
using Microsoft.EntityFrameworkCore;

namespace BE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsConstants.FrontendPolicyName, policy =>
                {
                    policy.WithOrigins(CorsConstants.AllowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var connectionString = builder.Configuration.GetConnectionString(DatabaseConstants.DefaultConnectionName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{DatabaseConstants.DefaultConnectionName}' is not configured.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddScoped<DatabaseHealthService>();
            builder.Services.AddScoped<MovieRepository>();
            builder.Services.AddScoped<MovieService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(CorsConstants.FrontendPolicyName);

            app.UseAuthorization();


            app.MapControllers();

            // Configure port from environment variable (for Render deployment)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Urls.Add($"http://+:{port}");

            await app.RunAsync();
        }
    }
}
