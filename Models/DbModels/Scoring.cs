using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.DbModels
{
    public class Scoring
    {
        public int ScoringId { get; set; }
        public int Score { get; set; }
        public string DispoId { get; set; }
        public DateTime DateOfRelevation { get; set; }
    }
}
