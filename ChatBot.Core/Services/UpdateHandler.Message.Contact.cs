using ChatBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    private async Task HandleMessageContactAsync(Message message)
    {
        var user = await dbContext.Set<Models.User>()
            .FindAsync(message.From.Id);

        user.Phone = message.Contact.PhoneNumber;

        dbContext.Set<Models.User>().Update(user);
        await dbContext.SaveChangesAsync();

        var markup = new ReplyKeyboardMarkup(
                KeyboardButton.WithRequestLocation("Lokatsiya yuborish"))
        {
            ResizeKeyboard = true
        };

        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: $"Lokatsiya yuboring!",
            replyMarkup: markup);
    }
}
