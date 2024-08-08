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
        builder.Services.AddScoped<IMessageService, MessageService>();


        var connection = builder.Configuration.GetConnectionString("AzureSQLDatabase");
        builder.Services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlServer(connection));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSignalR();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        app.MapControllers();
        app.MapHub<ChatHub>("chathub");

        app.Run();
    }
}