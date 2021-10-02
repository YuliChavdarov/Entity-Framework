using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.DTO.OutputModels
{
    public class DistrictOutputModel
    {
        public string Name { get; set; }
        public decimal AveragePricePerSquareMeter { get; set; }
        public int PropertiesCount { get; set; }
    }
}
