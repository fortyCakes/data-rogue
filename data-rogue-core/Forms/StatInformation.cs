namespace data_rogue_core.Forms.StaticForms
{
    public class FormStatInformation
    {
        public string statName;
        public int minStat;
        public int statValue;
        public int maxStat;

        public FormStatInformation(string statName, int minStat, int startStat, int maxStat)
        {
            this.statName = statName;
            this.minStat = minStat;
            statValue = startStat;
            this.maxStat = maxStat;
        }

        public void Increase()
        {
            if (statValue < maxStat)
            {
                statValue++;
            }
        }

        public void Decrease()
        {
            if (statValue > minStat)
            {
                statValue--;
            }
        }
    }
}