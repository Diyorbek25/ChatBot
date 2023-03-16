using ChatBot.Core.Contexts;
using DailyLeetcodeReminder.Core.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace DailyLeetcodeReminder.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBotClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        string botApiKey = configuration
            .GetSection("TelegramBot:ApiKey").Value;

        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(x => new TelegramBotClient(botApiKey));

        return services;
    }

    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString: configuration.GetConnectionString("SqlServer"));
        });

        return services;
    }

    public static IServiceCollection AddUpdateHandler(
        this IServiceCollection services)
    {
        services.AddTransient<UpdateHandler>();

        return services;
    }
}