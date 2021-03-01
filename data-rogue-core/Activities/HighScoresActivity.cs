using data_rogue_core.Systems.Interfaces;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class HighScoresActivity : StaticTextActivity
    {
        private ISaveSystem _saveSystem;

        public HighScoresActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, ISaveSystem saveSystem) : base(position, padding, activitySystem, "", true)
        {
            _saveSystem = saveSystem;

            var highScoreText = GetHighScoreText();

            Text = highScoreText;
        }

        private string GetHighScoreText()
        {
            var highScores = _saveSystem.GetHighScores().Take(10);

            var highScoreText = new StringBuilder();
            
            highScoreText.AppendLine();
            highScoreText.AppendLine(" High Scores:");
            highScoreText.AppendLine();
            int i = 1;
            foreach(var highScore in highScores)
            {
                highScoreText.AppendLine($" {i++}: {highScore.Name}: {highScore.Score}");
            }

            if (!highScores.Any())
            {
                highScoreText.AppendLine(" ( no high scores )");
            }

            return highScoreText.ToString();
        }
    }
}
