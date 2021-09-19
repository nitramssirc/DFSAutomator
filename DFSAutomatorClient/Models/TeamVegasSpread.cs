using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFSAutomatorClient.Models
{
    public class TeamVegasSpread
    {
        public string Time { get; set; }

        public string Team { get; set; }

        public string Opponent { get; set; }

        public decimal Line { get; set; }

        public decimal Over_Under { get; set; }

        public decimal ProjectedPoints { get; set; }

        public decimal RankValue
        {
            get
            {
                return ProjectedPoints - Math.Abs(Line) / 2;
            }
        }
    }
}
