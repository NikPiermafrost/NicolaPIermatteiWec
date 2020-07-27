using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Models.ViewModels
{
    public class FirstPageModel
    {
        public List<DistanceByProvince> DistanceByProvinces { get; set; }
        public List<TypeOfRelevation> TypeOfRelevations { get; set; }
        public List<TopTenByScore> TopTenByScores { get; set; }
        public List<TopTenDevicesByContact> TopTenDevices { get; set; }
    }
}
