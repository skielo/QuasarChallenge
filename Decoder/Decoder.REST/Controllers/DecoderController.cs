using Decoder.Logic;
using Decoder.REST.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;


namespace Decoder.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecoderController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public ShipDecoder decoder { get; set; }
        public DecoderController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            var sat = configuration.GetSection("satellites").Get<string>();
           decoder = new ShipDecoder(JsonConvert.DeserializeObject<Satellite[]>(sat));
            
        }

        // POST api/<DecoderController>/topsecret_split/satellite_name
        [HttpGet("topsecret_split")]
        public IActionResult Get()
        {
            (double x, double y) possition;
            string message;
            try
            {
                var distances = new double[decoder.Satellites.Length];
                var messages = new string[decoder.Satellites.Length][];
                var index = 0;
                foreach (var item in decoder.Satellites)
                {
                    var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, $"{item.Name}.json");

                    if (!System.IO.File.Exists(filePath))
                        return NotFound($"Not enough information to locate the vessel. Missing information about satellite: {item.Name}");

                    var dto = JsonConvert.DeserializeObject<SatelliteDTO>(System.IO.File.ReadAllText(filePath));

                    if (dto == null)
                        throw new ArgumentNullException("There was an issue trying to get the stored data. Please post the data again.");

                    distances[index] = dto.Distance;
                    messages[index] = dto.Message;
                    index++;
                }

                if(distances.Any(x => x == 0) || messages.Any(x => x == null))
                {
                    return NotFound("Not enough information to locate the vessel");
                }

                possition = decoder.GetLocation(distances);
                message = decoder.GetMessage(messages);

                if (string.IsNullOrEmpty(message) || (possition.x == 0 && possition.y == 0))
                {
                    return NotFound("Unable to locate  the vessel");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

            return Ok(new ResponseDTO
                    {
                        Possition = new PossitionDTO { X = possition.x, Y = possition.y },
                        Message = message
                    });
        }

        // POST api/<DecoderController>/topsecret_split/satellite_name
        [HttpPost("topsecret_split/{satellite_name}")]
        public IActionResult PostSatellite([FromBody] SatelliteDTO value, string satellite_name)
        {
            var message = new ErrorDTO();

            if (value == null)
                message.Errors.Add("The satellite information can't be null");
            if (string.IsNullOrEmpty(satellite_name))
                message.Errors.Add("Satellite name can't be null or empty.");
            if(!decoder.Satellites.Any(x => x.Name == satellite_name))
                message.Errors.Add("Satellite name doesn't match with the stored data.");
            if (message.Errors.Any())
                return BadRequest(message);

            try
            {
                value.Name = satellite_name;
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, $"{ satellite_name}.json");

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
            return Ok();
        }

        // POST api/<DecoderController>/topsecret
        [HttpPost("topsecret")]
        public IActionResult Post([FromBody] SatelliteDTO[] value)
        {
            (double x, double y) possition;
            string message;
            if (value.Length != decoder.Satellites.Length)
            {
                return BadRequest("Incorrect number of satellites.");
            }
            
            try
            {
                var distances = new double[value.Length];
                var messages = new string[value.Length][];

                for (int i = 0; i < value.Length; i++)
                {
                    distances[i] = value[i].Distance;
                    messages[i] = value[i].Message;
                }

                possition = decoder.GetLocation(distances);
                message = decoder.GetMessage(messages);

                if (string.IsNullOrEmpty(message) || (possition.x == 0 && possition.y == 0))
                {
                    return NotFound("Unable to locate  the vessel");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }


            return Ok(new
             ResponseDTO {
                Possition = new PossitionDTO { X = possition.x, Y = possition.y },
                Message = message
            });
        }
    }
}
