using Decoder.Logic;
using Decoder.REST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Decoder.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecoderController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public ShipDecoder decoder { get; set; }
        public DecoderController(IConfiguration configuration)
        {
            var sat = configuration.GetSection("satellites").Get<string>();
           decoder = new ShipDecoder(JsonConvert.DeserializeObject<Satellite[]>(sat));
            
        }
        // POST api/<DecoderController>/topsecret_split/satellite_name
        [HttpPost("topsecret_split/{satellite_name}")]
        public string Get([FromBody] SatelliteDTO value, string satellite_name)
        {
            return "value";
        }

        // POST api/<DecoderController>/topsecret
        [HttpPost("topsecret")]
        public IActionResult Post([FromBody] SatelliteDTO[] value)
        {
            if(value.Length != 3)
            {
                return BadRequest();
            }
            var distances = new double[value.Length];
            var messages = new string[value.Length][];

            for (int i = 0; i < value.Length; i++)
            {
                distances[i] = value[i].Distance;
                messages[i] = value[i].Message;
            }

            var possition = decoder.GetLocation(distances);
            var message = decoder.GetMessage(messages);

            if(string.IsNullOrEmpty(message) || (possition.x == 0 && possition.y == 0))
            {
                return NotFound();
            }

            return Ok(new
            {
                possition = new
                {
                    possition.x,
                    possition.y
                },
                message
            });
        }
    }
}
