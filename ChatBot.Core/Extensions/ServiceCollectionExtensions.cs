using ChatBot.Core.Contexts;
using ChatBot.Core.Jobs;
using ChatBot.Core.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Telegram.Bot;

namespace ChatBot.Core.Extensions;

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

    public static IServiceCollection AddJobs(
        this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionScopedJobFactory();

            var dailyReminderJobKey = new JobKey(nameof(DailyMessageJob));

            q.AddJob<DailyMessageJob>(opts =>
            {
                opts.WithIdentity(dailyReminderJobKey);
            });

            q.AddTrigger(opts => opts
                .ForJob(dailyReminderJobKey)
                .WithIdentity($"{dailyReminderJobKey.Name}-trigger")
                .WithCronSchedule("0 0 */2 * * ?")
            );
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}