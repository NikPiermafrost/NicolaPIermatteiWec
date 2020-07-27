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
        [MaxLength(50)]
        public string DispoId { get; set; }
        [Required]
        [MaxLength(50)]
        public string DispoContactId { get; set; }
        [Required]
        public int Prox { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public int Distance { get; set; }
        [Required]
        public DateTime DateContacts{ get; set; }
    }
}
