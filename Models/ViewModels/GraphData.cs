using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.ViewModels
{
    public class GraphData
    {
        public string Province { get; set; }
        public DateTime DateEvent { get; set; }
        public int HowMany { get; set; }
        public string DateParsed => DateEvent.ToShortDateString();
    }
}
