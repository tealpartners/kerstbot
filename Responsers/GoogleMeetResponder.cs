using MargieBot;
using System.Text.RegularExpressions;

namespace Kerstbot.Responsers;

public class GoogleMeetResponder : IResponder
{
    public bool CanRespond(ResponseContext context)
    {
        var respond = !context.BotHasResponded
            && !context.Message.User.IsSlackbot
            && (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM)
            && context.Message.Text.ToLower().Contains("meet.google.com");

        return respond;
    }

    public BotMessage GetResponse(ResponseContext context)
    {
        var messageText = context.Message.Text;
        var pattern = @"<https?:\/\/([^\|\>]*)(\|[^>]*)?>";
        var m = Regex.Match(messageText, pattern);

        if (m.Success)
        {
            var url = "https://" + m.Groups[1].Value;
            Utils.OpenUrl(url);

            return new BotMessage { Text = "Ho, ho, ho! It's time to get jolly on your naughty asses! (" + url + ")" };
        }

        return new BotMessage { Text = "Sorry, I could not locate a meeting URL in your message..." };
    }
}
