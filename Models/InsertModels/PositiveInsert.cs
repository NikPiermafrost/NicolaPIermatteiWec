using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.InsertModels
{
    public class PositiveInsert
    {
        [Required]
        public string DispoId { get; set; }
        [Required]
        public bool Positive { get; set; }
        [Required]
        public DateTime DatePositive { get; set; }
    }
}
