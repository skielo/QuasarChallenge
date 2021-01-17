using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decoder.REST.Models
{
    public class SatelliteDTO
    {
        public string Name { get; set; }
        public double Distance { get; set; }
        public string[] Message { get; set; }
    }
}
