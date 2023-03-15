﻿using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace DailyLeetcodeReminder.Core.Services;

public static class ServiceHelper
{
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
