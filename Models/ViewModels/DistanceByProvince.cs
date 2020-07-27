using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.ViewModels
{
    public class DistanceByProvince
    {
        public string Province { get; set; }
        public int LessThanOne { get; set; }
        public int FromOneToHalf { get; set; }
        public int FromHalfToTwo { get; set; }
        public int FromTwoToHalf { get; set; }
        public int MoreThanTwoHalf { get; set; }
    }
}
