using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFSAutomatorClient.Models
{
    public class Player
    {
        public string player_id { get; set; }
        public string team { get; set; }
        public string opp { get; set; }
        public string pos { get; set; }
        public string name { get; set; }
        public decimal? fpts { get; set; }
        public decimal? proj_own { get; set; }
        public string smash { get; set; }
        public string value_percent { get; set; }
        public decimal? ceil { get; set; }
        public decimal? floor { get; set; }
        public decimal? min_exposure { get; set; }
        public decimal? max_exposure { get; set; }
        public decimal? rg_value { get; set; }
        public int? salary { get; set; }

        public decimal? custom { get; set; }

        public string rg_id { get; set; }

        public string partner_id { get; set; }
    }
}
