using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.DbModels
{
    public class PositiveTable
    {
        public string DispoId { get; set; }
        public bool Positive { get; set; }
        public DateTime DatePositive { get; set; }
    }
}
