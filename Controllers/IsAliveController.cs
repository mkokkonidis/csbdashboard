﻿using Microsoft.AspNetCore.Http;
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



        // GET /<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
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
