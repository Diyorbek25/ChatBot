using ChatBot.Core.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    public async Task HandleMessageTextAsync(Message message)
    {
        var command = message.Text.Split(' ').First().Substring(1);

        try
        {
            var task = command switch
            {
                "start" => HandleStartCommandAsync(message),
                "userId" => HandleUserIdCommandAsync(message),
                "send" => HandleSendCommandAsync(message),
                "admin" => HandleAdminCommandAsync(message),
                "register" => HandleRegisterCommandAsync(message),
                "map" => HandleMapCommandAsync(message),
                _ => HandleNotAvailableCommandAsync(message)
            };

            await task;
        }
        catch (Exception exception)
        {
            this.logger.LogError(exception.Message);

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Failed to handle your request. Please try again");

            return;
        }
    }

    private async Task HandleMapCommandAsync(Message message)
    {
        var user = await dbContext.Set<Models.User>()
            .Include(user => user.SelectedUsers)
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        foreach (var item in user.SelectedUsers)
        {

            var selectedUser = dbContext.Set<Models.User>()
                .Include(user => user.Address)
                .FirstOrDefault(user => user.Id == item.SelectedUserId);

            if (selectedUser == null)
            {
                return;
            }

            telegramBotClient.SendLocationAsync(
                chatId: message.From.Id,
                latitude: (double)selectedUser.Address?.Latitude,
                longitude: (double)selectedUser.Address.Longitude);
        }
    }

    private async Task HandleRegisterCommandAsync(Message message)
    {
        string fullName = message.Text.Substring(9);
        var user = await dbContext.Set<Models.User>()
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        user.FullName = fullName.TrimStart().TrimEnd();

        await dbContext.SaveChangesAsync();
        
        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: "Ma'lumotlaringiz saqlandi.");
    }

    private async Task HandleAdminCommandAsync(Message message)
    {
        var adminUser = await dbContext
            .Set<Models.User>()
            .Include("SelectedUsers")
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        if (adminUser.Role == Role.Admin && adminUser.SelectedUsers is not null)
        {
            var users = dbContext
                .Set<Models.User>()
                .Where(admin => adminUser.SelectedUsers
                .Select(user => user.SelectedUserId).Contains(admin.Id));

            foreach (var user in users)
            {
                user.Role = Role.Admin;
            }

            await dbContext.SaveChangesAsync();
        }
        return;
    }

    private async Task HandleStartCommandAsync(Message message)
    {
        var telegramUser = message.From;

        var user = await dbContext.Set<Models.User>()
            .FindAsync(telegramUser.Id);

        if (user is null)
        {
            user = new Models.User()
            {
                Id = telegramUser.Id,
                UserName = telegramUser.Username,
                FirstName = telegramUser.FirstName,
                LastName = telegramUser.LastName
            };

            dbContext.Set<Models.User>().Add(user);
            await dbContext.SaveChangesAsync();

            var markup = new ReplyKeyboardMarkup(
                KeyboardButton.WithRequestContact("Contact yuborish"));

            markup.ResizeKeyboard = true;

            await this.telegramBotClient.SendTextMessageAsync(
                chatId: message.From.Id,
                text: "Contact yuboring!",
                replyMarkup: markup);

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


    private async Task HandleUserIdCommandAsync(Message message)
    {
        var datas = message.Text.Split(' ');
        long userId = long.Parse(datas[1]);

        var user = await dbContext
            .Set<Models.User>()
            .Include("SelectedUsers")
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        user.SelectedUsers.Add(new SelectedUser() { SelectedUserId = userId });

        dbContext.Set<Models.User>().Update(user);

        await dbContext.SaveChangesAsync();

        string messageText = "";
        if (user.SelectedUsers.Count <= 1)
        {
            messageText = "<b>Foydalnuvchi tanlandi. Unga xabar yuboring. </b>\n\n" +
                "(adminlikka tayinlash uchun /admin kommandasini yuboring).\n" +
                "(/map - Tanlangan foydalanuvchi Manzilini olish)";
        }
        else
        {
            messageText = "<b>Foydalanuvchilar tanlandi. Ularga xabar yuboring. </b>\n\n" +
                "(ularni adminlikka tayinlash uchun /admin kommandasini yuboring).\n" +
                "(/map - Tanlangan foydalanuvchilar Manzillarini olish)";
        }

        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: messageText,
            parseMode: ParseMode.Html);
    }

    private async Task HandleSendCommandAsync(Message message)
    {
        var users = dbContext
            .Set<Models.User>()
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
