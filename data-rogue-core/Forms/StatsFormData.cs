﻿using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;

namespace data_rogue_core.Forms.StaticForms
{
    public class StatsFormData : SubSelectableFormData
    {
        public int MaxTotalStat;

        public List<FormStatInformation> Stats => (List<FormStatInformation>)Value;

        public int CurrentTotalStat {
            get
            {
                return Stats.Sum(s => s.statValue);
            }
        }

        public StatsFormData(string name, int order, int maxTotalStat, List<FormStatInformation> stats) : base(name, FormDataType.StatArray, stats, order)
        {
            HasSubFields = true;
            MaxTotalStat = maxTotalStat;
        }

        public override List<string> GetSubItems()
        {
            return Stats.Select(s => s.statName).ToList();
        }

        public void ChangeStat(string subItem, bool increase)
        {
            var stat = Stats.Single(s => s.statName == subItem);

            if (increase)
            {
                if (CurrentTotalStat == MaxTotalStat)
                {
                    return;
                }

                stat.Increase();
            }
            else
            {
                stat.Decrease();
            }
        }

        public int GetStat(string statName)
        {
            return Stats.Single(s => s.statName == statName).statValue;
        }
    }
}