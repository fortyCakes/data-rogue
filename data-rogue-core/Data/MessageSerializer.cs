using System.Drawing;
using System.Text.RegularExpressions;

namespace data_rogue_core.Data
{
    public class MessageSerializer
    {
        public static string Serialize(Message message)
        {
            return message.ToString();
        }

        public static Message Deserialize(string text)
        {
            var match = Regex.Match(text, "(.*):(.*)");

            var color = match.Groups[1].Value;

            var messageText = match.Groups[2].Value;

            return new Message {Text = messageText, Color = ColorTranslator.FromHtml(color)};
        }
    }
}