using NicolaPIermatteiWec.Models.InsertModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models
{
    public class BulkInsertResponse
    {
        public List<DailyInsert> DailyInserted { get; set; }
        public List<DailyInsert> DailyNotInserted{ get; set; }
        public List<PositiveInsert> PositiveInserted { get; set; }
        public List<PositiveInsert> PositiveNotInserted { get; set; }
    }
}
