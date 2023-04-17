using ChatBot.Core.Contexts;
using ChatBot.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    private string path = @"D:\Telegram Bot Projects\ChatBot\ChatBot.Core\Logs\log.txt";

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
        System.IO.File.AppendAllText(path, $"{DateTime.Now} - {update.ToString()}\n");

        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(update.Message),
            UpdateType.InlineQuery => HandleInlineQueryAsync(update),
            _ => HandleNotAvailableCommandAsync(update.Message)
        };

        await handler;
    }

    
    private async Task HandleNotAvailableCommandAsync(Message message)
    {
        if (message is null) 
        { 
            return;
        }

        string messageText = message.Text;

        var user = dbContext.Set<Models.User>()
            .Where(user => user.Role == Role.Admin)
            .Include("SelectedUsers")
            .FirstOrDefault(user => user.Id == message.From.Id);

        if (user != null && user.SelectedUsers is not null)
        {
            var userData = $"<pre>Admindan: {message.From.FirstName} {message.From.LastName}</pre>";

            foreach (var selectedUser in user.SelectedUsers)
            {
                await telegramBotClient.SendTextMessageAsync(
                    chatId: selectedUser.SelectedUserId,
                    text: $"{userData}\n\n" + $"<b> {messageText}</b>",
                    parseMode: ParseMode.Html);

                dbContext.Set<SelectedUser>().Remove(selectedUser);
            }

            await dbContext.SaveChangesAsync();

            await telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Xabar yuborildi!");

            return;
        }
        if (user == null)
        {
            var adminUsers = dbContext
                .Set<Models.User>()
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
}
