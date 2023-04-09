
using ChatBot.Core.Models.JsonModels;
using System.Text.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace DailyLeetcodeReminder.Core.Services;

public static class ServiceHelper
{
    public static async Task<List<string>> GetAddressAsync(double lat, double lng)
    {
        var apiKey = "AIzaSyBq0twb915arXgAKwJXr3XDocU_jt2TgzU";
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={apiKey}";

        using var client = new HttpClient();
        
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var address = await response.Content.ReadAsStringAsync();

        var root = JsonSerializer.Deserialize<Root>(address);
        
        return root.results
            .Select(res => res.formatted_address)
            .Take(3).ToList();
    }

    public static InlineKeyboardMarkup GenerateButtons(int page)
    {
        var buttons = new List<List<InlineKeyboardButton>>();

        for (int index = 0; index <= page; index++)
        {
            if (index % 3 == 0)
                buttons.Add(new List<InlineKeyboardButton>());

            buttons[index / 3].Add(
                new InlineKeyboardButton($"{index + 1}")
                {
                    CallbackData = $"{index + 1}"
                }
            );
        }

        return new InlineKeyboardMarkup(buttons);
    }

    /*public static string TableBuilder(List<Challenger> challengers)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Rating");
        builder.AppendLine($"<pre>|{new string('-', 22)}|{new string('-', 10)}|");

        builder.AppendLine(String.Format("| {0, -20} | {1, -9}|",
            "UserName", "Total"));

        builder.AppendLine($"|{new string('-', 22)}|{new string('-', 10)}|");

        foreach (var challanger in challengers)
        {
            builder.AppendLine(String.Format("| {0, -20} | {1, -9}|",
                challanger.LeetcodeUserName, challanger.TotalSolvedProblems));
        }

        return builder.ToString() + "</pre>";
    }*/
}
