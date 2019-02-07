using System.Drawing;

namespace data_rogue_core.Data
{
    public class Message
    {
        public string Text { get; set; }
        public Color Color { get; set; }

        public override string ToString()
        {
            return ColorTranslator.ToHtml(Color) + ":" + Text;
        }
    }
}