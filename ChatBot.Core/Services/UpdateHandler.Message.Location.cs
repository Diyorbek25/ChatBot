using Azure.Core;
using ChatBot.Core.Models;
using DailyLeetcodeReminder.Core.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Yandex.Geocoder;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    private async Task HandlerMessageLocationAsync(Message message)
    {
        var location = message.Location;

        var user = await dbContext.Set<Models.User>()
            .Include("Address")
            .FirstOrDefaultAsync(user => user.Id == message.From.Id);

        var addressDatas = await ServiceHelper
            .GetAddressAsync(location.Latitude, location.Longitude);

        if (user.AddressId == null)
        {
            user.Address = new Models.Address
            {
                Longitude = location.Longitude,
                Latitude = location.Latitude,
                AddressData = String.Join(",\n", addressDatas)
            };
        }
        {
            user.Address.Longitude = location.Longitude;
            user.Address.Latitude = location.Latitude;
            user.Address.AddressData = String.Join(",\n", addressDatas);
        }

        var re = new ReverseGeocoderRequest()
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };

        await dbContext.SaveChangesAsync();

        await telegramBotClient.SendTextMessageAsync(
            chatId: message.From.Id,
            text: "Ism familiyangizni quyidagi formatda yuboring.\n" +
            "/register Palonchiyev Pistonchi",
            replyMarkup: new ReplyKeyboardRemove());
    }
}
