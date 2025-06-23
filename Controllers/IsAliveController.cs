using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authentication;
using CSBDashboardServer.Helpers;

namespace CSBDashboardServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IsAliveController : ControllerBase
    {
        public const bool Verbose = false;
        private const int Servers = 4;
        private const int Minutes = 15;
        static private int skip = 0;


        // GET /<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? reset)
        {
            if (reset == true) skip = 0;

            if(skip-->0) return StatusCode(200,"SKIP COUNTER: " +  skip + "; use ?reset=true if you want an immediate answer");
            
            skip = Servers * Minutes;

            try
            {
                var isAlive = Environment.GetEnvironmentVariable("ISALIVE");
                return StatusCode(200, IsAliveHelper.IsAlive(isAlive));
            } catch(Exception exc) {
                return StatusCode(500, exc.Message);
            }
        }



    }
}
