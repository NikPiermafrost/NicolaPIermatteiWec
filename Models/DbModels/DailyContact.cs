using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.DbModels
{
    public class DailyContact
    {
        public int DailyContactId { get; set; }
        public string DispoId { get; set; }
        public string DispoContactId { get; set; }
        public float Prox { get; set; }
        public string Province { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Distance { get; set; }
        public string DateContact { get; set; }
    }
}
