using ChatBot.Core.Models;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    private async Task HandleVoiceMessageAsync(Message message)
    {
        var adminUser = await dbContext
            .Set<Models.User>()
            .Include("SelectedUsers")
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        if (adminUser.Role == Role.Admin)
        {
            if (adminUser.SelectedUsers is not null)
            {
                foreach (var user in adminUser.SelectedUsers)
                {
                    await telegramBotClient.SendVoiceAsync(
                        chatId: user.SelectedUserId,
                        voice: message.Voice.FileId);
                }
            }
            else
            {
                var users = dbContext
                    .Set<Models.User>()
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
}
