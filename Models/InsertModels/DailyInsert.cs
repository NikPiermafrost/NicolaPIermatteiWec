using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.InsertModels
{
    public class DailyInsert
    {
        [Required]
        public string DispoId { get; set; }
        [Required]
        public string DispoContactId { get; set; }
        [Required]
        public int Prox { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        [Range(-90, 90)]
        public float Latitude { get; set; }
        [Required]
        [Range(-90, 90)]
        public float Longitude { get; set; }
        [Required]
        public int Distance { get; set; }
        [Required]
        public string DateContact { get; set; }
    }
}
