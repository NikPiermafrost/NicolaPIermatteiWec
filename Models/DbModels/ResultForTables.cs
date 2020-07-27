using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.DbModels
{
    public class ResultForTables
    {
        public List<DailyContact> DailyContacts { get; set; }
        public List<PositiveTable> PositiveTables { get; set; }
        public List<Scoring> Scorings { get; set; }
    }
}
