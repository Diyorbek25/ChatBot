using ChatBot.Core.Contexts;
using Quartz;
using Telegram.Bot;

namespace ChatBot.Core.Jobs;

public class DailyMessageJob : IJob
{
    private ITelegramBotClient botClient;
    private readonly AppDbContext dbContext;
    private ILogger<DailyMessageJob> logger;

    public DailyMessageJob(
        ITelegramBotClient botClient,
        AppDbContext dbContext,
        ILogger<DailyMessageJob> logger)
    {
        this.botClient = botClient;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var users = dbContext.Set<Models.User>();

            foreach (var user in users)
            {
                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "Bot Ishlayapti");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }
}
