using System;
using System.Text;
using MargieBot;

namespace Kerstbot.Responsers
{
    public class HelloResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            return !context.BotHasResponded
                   && !context.Message.User.IsSlackbot
                   && (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM)
                   && context.Message.Text.ToLower().Contains("hello");
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            builder.Append("Hello ").Append(context.Message.User.FormattedUserID);

            return new BotMessage { Text = builder.ToString() };
        }
    }
}