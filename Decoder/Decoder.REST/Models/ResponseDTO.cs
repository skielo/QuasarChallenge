using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decoder.REST.Models
{
    public class ResponseDTO
    {
        public PossitionDTO Possition { get; set; }
        public string Message { get; set; }
    }

    public class PossitionDTO
    {
        public double X { get; set; }
        public double Y { get; set; }

    }
}
