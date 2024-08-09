using Microsoft.EntityFrameworkCore;
using TestChat.Server.Data;
using TestChat.Server.Hubs;
using TestChat.Server.Services;

namespace TestChat.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddScoped<ITextAnalyticsService, TextAnalyticsService>();
        builder.Services.AddScoped<IPersistenceService, PersistenceService>();


        var connection = builder.Configuration.GetConnectionString("AzureSQLDatabase");
        builder.Services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlServer(connection));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (builder.Environment.IsDevelopment())
            builder.Services.AddSignalR();
        else
            builder.Services.AddSignalR().AddAzureSignalR();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            dbContext.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var clientUrl = builder.Configuration["ClientUrl"];
        app.UseCors(options => options.WithOrigins(clientUrl).AllowAnyHeader().AllowAnyMethod());

        app.UseHttpsRedirection();

        app.MapControllers();
        app.MapHub<ChatHub>("chathub");

        app.Run();
    }
}