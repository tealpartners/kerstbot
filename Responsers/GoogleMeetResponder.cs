using System;
using MargieBot;
using System.Linq;

namespace Kerstbot.Responsers
{
    public class GoogleMeetResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            var respond = context.Message.MentionsBot
                && !context.BotHasResponded
                && context.Message.Text.ToLower().Contains("meet.google.com");

            return respond;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var parts = context.Message.Text.Split(" ").ToList();
            var meetPart = parts.FirstOrDefault(p => p.Contains("meet.google.com"));

            if (meetPart == null)
            {
                return new BotMessage { Text = "Kon url niet vinden :|" };
            }

            if (meetPart.StartsWith("<"))
            {
                meetPart = meetPart.Substring(1);
            }
            if (meetPart.EndsWith(">"))
            {
                meetPart = meetPart.Remove(meetPart.Length - 1);
            }

            Utils.OpenUrl(meetPart);

            return new BotMessage { Text = "Ho, ho, ho! It's time to get jolly on your naughty asses! (" + meetPart + ")" };
        }
    }
}