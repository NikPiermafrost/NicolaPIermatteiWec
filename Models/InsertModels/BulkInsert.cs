using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.InsertModels
{
    public class BulkInsert
    {
        public List<DailyInsert> DailyInserts { get; set; }
        public List<PositiveInsert> PositiveInserts { get; set; }
    }
}
