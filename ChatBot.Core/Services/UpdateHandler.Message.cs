using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    public async Task HandleMessageAsync(Message message)
    {
        var handler = message.Type switch
        {
            MessageType.Text => HandleMessageTextAsync(message),
            MessageType.Voice => HandleVoiceMessageAsync(message),
            MessageType.Contact => HandleMessageContactAsync(message),
            MessageType.Location => HandlerMessageLocationAsync(message),
            _ => HandleNotAvailableCommandAsync(message)
        };

        await handler;

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
    }
}
