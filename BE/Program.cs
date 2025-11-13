
using BE.Constants;
using BE.Services;
using Npgsql;

namespace BE
{
    public class Program
    {
        public static void Main(string[] args)
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

            builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
            builder.Services.AddScoped<DatabaseHealthService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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

            app.Run();
        }
    }
}
