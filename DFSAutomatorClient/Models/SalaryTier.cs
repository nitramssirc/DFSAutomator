using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFSAutomatorClient.Models
{
    public class SalaryTier
    {
        public decimal Pct;
        public int Max;
        public int Min;
        public string Pos;

        public SalaryTier(string pos, int min, int max, decimal pct)
        {
            Pos = pos;
            Min = min;
            Max = max;
            Pct = pct;
        }
    }
}
