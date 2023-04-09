using ChatBot.Core.Extensions;
using ChatBot.Core.Middlewares;
using Telegram.Bot;


namespace ChatBot.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services
            .AddControllers()
            .AddNewtonsoftJson();

        builder.Services
            .AddDbContexts(builder.Configuration)
            .AddUpdateHandler()
            .AddTelegramBotClient(builder.Configuration)
            .AddJobs();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
        //}

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.MapControllers();

        SetWebHook(app, builder.Configuration);

        app.Run();
    }
    private static void SetWebHook(
            IApplicationBuilder builder,
            IConfiguration configuration)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var baseUrl = configuration.GetSection("TelegramBot:BaseAddress").Value;
            var webhookUrl = $"{baseUrl}/bot";

            var webhookInfo = botClient.GetWebhookInfoAsync().Result;

            if (webhookInfo is null || webhookInfo.Url != webhookUrl)
            {
                botClient.SetWebhookAsync(webhookUrl).Wait();
            }
        }
    }
}