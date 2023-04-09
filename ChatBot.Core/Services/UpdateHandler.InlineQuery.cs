using ChatBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace ChatBot.Core.Services;

public partial class UpdateHandler
{
    private async Task HandleInlineQueryAsync(Update update)
    {
        string searchText = update.InlineQuery.Query.ToLower();

        var users = dbContext
            .Set<Models.User>()
            .Where(user => (user.FullName.ToLower().StartsWith(searchText) || user.FirstName.ToLower().StartsWith(searchText))
            && user.Role == Role.User);


        var results = new List<InlineQueryResult>();

        foreach (var user in users)
        {
            results.Add(new InlineQueryResultArticle(
                id: user.Id.ToString(),
                title: user.FullName ?? user.FirstName + " " + user.LastName,
                inputMessageContent: new InputTextMessageContent(
                    $"/userId {user.Id} \n{user.FullName}")));
        }

        await telegramBotClient.AnswerInlineQueryAsync(
            inlineQueryId: update.InlineQuery.Id,
            results: results,
            isPersonal: true,
            cacheTime: 0);
    }
}
