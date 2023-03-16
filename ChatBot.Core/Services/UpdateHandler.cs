using ChatBot.Core.Contexts;
using ChatBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace DailyLeetcodeReminder.Core.Services;

public class UpdateHandler
{
    private readonly ITelegramBotClient telegramBotClient;
    private readonly ILogger<UpdateHandler> logger;
    private readonly AppDbContext dbContext;

    public UpdateHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<UpdateHandler> logger,
        AppDbContext dbContext)
    {
        this.telegramBotClient = telegramBotClient;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task UpdateHandlerAsync(Update update)
    {
        
        var handler = update.Type switch
        {
            UpdateType.Message => HandleCommandAsync(update.Message),
            UpdateType.InlineQuery => HandleInlineQueryAsync(update),
            _ => HandleNotAvailableCommandAsync(update.Message)
        };

        await handler;
    }

    private async Task HandleInlineQueryAsync(Update update)
    {
        string searchText = update.InlineQuery.Query.ToLower();

        var users = dbContext
            .Set<ChatBot.Core.Models.User>()
            .Where(user => user.FirstName.ToLower().StartsWith(searchText) && user.Role == Role.User);


        var results = new List<InlineQueryResult>();

        foreach (var user in users)
        {
            results.Add(new InlineQueryResultArticle(
                id: user.Id.ToString(),
                title: user.UserName ?? user.FirstName + " " + user.LastName,
                inputMessageContent: new InputTextMessageContent(
                    $"/userId {user.Id} \n{user.FirstName} {user.LastName}")));
        }

        await telegramBotClient.AnswerInlineQueryAsync(
            inlineQueryId: update.InlineQuery.Id,
            results: results,
            isPersonal: true,
            cacheTime: 0);
    }

    public async Task HandleCommandAsync(Message message)
    {

        if (message.Voice is not null)
        {
            await HandleVoiceMessageAsync(message);
        }

        if (message.Text == null)
        {
            return;
        }

        var command = message.Text.Split(' ').First().Substring(1);

        try
        {
            var task = command switch
            {
                "start" => HandleStartCommandAsync(message),
                "userId" => HandleUserIdCommandAsync(message),
                "send" => HandleSendCommandAsync(message),
                "admin" => HandleAdminCommandAsync(message),
                _ => HandleNotAvailableCommandAsync(message)
            };

            await task;
        }
        /*catch (AlreadyExistsException exception)
        {
            this.logger.LogError(exception.Message);

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Siz allaqachon ro'yxatdan o'tgansiz");

            return;
        }
        catch (NotFoundException exception)
        {
            this.logger.LogError(exception.Message);

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Kechirasiz usernameni tekshirib qayta urining, username topilmadi");

            return;
        }
        catch (DuplicateException exception)
        {
            this.logger.LogError(exception.Message);

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Sizning telegram yoki leetcode profilingiz ro'yxatdan o'tgan");

            return;
        }*/
        catch (Exception exception)
        {
            this.logger.LogError(exception.Message);

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Failed to handle your request. Please try again");

            return;
        }
    }

    private async Task HandleVoiceMessageAsync(Message message)
    {
        var adminUser = await dbContext
            .Set<ChatBot.Core.Models.User>()
            .FindAsync(message.From.Id);

        if (adminUser.Role == Role.Admin)
        {
            if (adminUser.SelectUserId is not null)
            {
                await telegramBotClient.SendVoiceAsync(
                    chatId: adminUser.SelectUserId,
                    voice: message.Voice.FileId);
            } else
            {
                var users = dbContext
                    .Set<ChatBot.Core.Models.User>()
                    .Where(user => user.Role == Role.User);

                foreach (var user in users)
                {
                    try
                    {
                        await telegramBotClient.SendVoiceAsync(
                            chatId: user.Id,
                            voice: message.Voice.FileId);
                    }
                    catch (Exception)
                    {
                        dbContext.Remove(user);
                    }
                }
                await dbContext.SaveChangesAsync();

                await telegramBotClient.SendTextMessageAsync(
                    chatId: message.From.Id,
                    text: "Xabar yuborildi");
            }
        }
        return;
    }

    private async Task HandleAdminCommandAsync(Message message)
    {
        var adminUser = await dbContext
            .Set<ChatBot.Core.Models.User>()
            .FindAsync(message.From.Id);
        if (adminUser.Role == Role.Admin && adminUser.SelectUserId is not null)
        {
            var user = await dbContext
                .Set<ChatBot.Core.Models.User>()
                .FindAsync(adminUser.SelectUserId);
            user.Role = Role.Admin;

            dbContext.Set<ChatBot.Core.Models.User>().Update(user);
            await dbContext.SaveChangesAsync();
        }
        return;
    }

    private async Task HandleStartCommandAsync(Message message)
    {
        var telegramUser = message.From;

        var user = await dbContext.Set<ChatBot.Core.Models.User>()
            .FindAsync(telegramUser.Id);

        if (user is null)
        {
            user = new ChatBot.Core.Models.User()
            {
                Id = telegramUser.Id,
                UserName = telegramUser.Username,
                FirstName = telegramUser.FirstName,
                LastName = telegramUser.LastName
            };

            dbContext.Set<ChatBot.Core.Models.User>().Add(user);
            await dbContext.SaveChangesAsync();
            return;
        }
        if (user.Role == Role.Admin)
        {
            await this.telegramBotClient.SendTextMessageAsync(
                    chatId: message.From.Id,
                    text: "Assalomu alaykum!\nBotga start bosgan userlar xabar " +
                    "yuborish uchun /send kommandasidan keyin xabar yozib yuboring.");
        }
    }

    private async Task HandleNotAvailableCommandAsync(Message message)
    {
        if (message is null) 
        { 
            return;
        }

        string messageText = message.Text;

        var user = dbContext.Set<ChatBot.Core.Models.User>()
            .Where(user => user.Role == Role.Admin)
            .FirstOrDefault(user => user.Id == message.From.Id);

        if (user != null && user.SelectUserId is not null)
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: user.SelectUserId,
                text: messageText);

            user.SelectUserId = null;

            await dbContext.SaveChangesAsync();

            await telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Xabar yuborildi!");

            return;
        }
        if (user == null)
        {
            var adminUsers = dbContext
                .Set<ChatBot.Core.Models.User>()
                .Where(user => user.Role == Role.Admin);

            var userData = $"<pre>kimdan: {message.From.FirstName} {message.From.LastName}</pre>";

            foreach (var adminUser in adminUsers)
            {
                await telegramBotClient.SendTextMessageAsync(
                    chatId: adminUser.Id,
                    text: $"{userData}\n\n" + $"<b> {messageText}</b>",
                    parseMode: ParseMode.Html);
            }
            return;
        }

        await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Mavjud bo'lmagan komanda kiritildi. " +
                "Tekshirib ko'ring.");
    }

    private async Task HandleUserIdCommandAsync(Message message)
    {
        var datas = message.Text.Split(' ');
        long userId = long.Parse(datas[1]);

        var user = await dbContext
            .Set<ChatBot.Core.Models.User>()
            .FindAsync(message.From.Id);

        user.SelectUserId = userId;
        dbContext.Set<ChatBot.Core.Models.User>().Update(user);

        await dbContext.SaveChangesAsync();

        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: "Xabar yozing!");
    }

    private async Task HandleSendCommandAsync(Message message)
    {
        var users = dbContext
            .Set<ChatBot.Core.Models.User>()
            .Where(user => user.Role == Role.User);

        string messageText = message.Text.Substring(5);

        foreach (var user in users)
        {
            try
            {
                await telegramBotClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: messageText);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
        await dbContext.SaveChangesAsync();

        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: "Xabar yuborildi");
    }
}
