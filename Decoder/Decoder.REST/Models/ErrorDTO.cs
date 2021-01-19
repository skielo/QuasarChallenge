using System;using System.Collections.Generic;

namespace Decoder.REST.Models
{
    public class ErrorDTO
    {
        public ErrorDTO()
        {
            Errors = new List<string>();
        }
        public List<string> Errors { get; set; }

    }
}
