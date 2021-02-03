using data_rogue_core.Systems.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace data_rogue_core.Activities
{
    public class HighScoresActivity : StaticTextActivity
    {
        private ISaveSystem _saveSystem;

        public HighScoresActivity(IActivitySystem activitySystem, ISaveSystem saveSystem) : base(activitySystem, "", true)
        {
            _saveSystem = saveSystem;

            var highScoreText = GetHighScoreText();

            var highScoresActivity = new StaticTextActivity(activitySystem, highScoreText);
        }

        private string GetHighScoreText()
        {
            var highScores = _saveSystem.GetHighScores().Take(10);

            var highScoreText = new StringBuilder();

            highScoreText.AppendLine("High Scores:");
            highScoreText.AppendLine();
            foreach(var highScore in highScores)
            {
                highScoreText.AppendLine($"{highScore.Name}: {highScore.Score}");
            }

            if (!highScores.Any())
            {
                highScoreText.AppendLine("( no high scores )");
            }

            return highScoreText.ToString();
        }
    }
}
