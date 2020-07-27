using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.ViewModels
{
    public class TypeOfRelevation
    {
        public string Province { get; set; }
        public int NumOfContacts { get; set; }
        public int NumOfPositives { get; set; }
        public int Proximity { get; set; }
    }
}
