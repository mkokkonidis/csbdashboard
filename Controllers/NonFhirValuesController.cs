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
    public class NonFhirValuesController : ControllerBase
    {
        public const bool Verbose = false;
        


        // GET /<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var auth = Request.Headers.Authorization.ToString();
            var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); // e.g., https://test-retention.biomed.ntua.gr/api/


            if(id == 0)
            {
                id =72;
                apiBaseUrlSlash = "https://staging-retention.biomed.ntua.gr/api/";
                auth = ""; // "Bearer ...";
            }

            Func<string, Task<object>> O = async (spec) => await Task.Run(() => FHIRHelper.CompactFHIRObservations(auth, apiBaseUrlSlash, id, spec));
            Func<string, object, Task<object>> PNO =  (path, data) => NonFHIRHelper.NonFHIRObservations(auth, apiBaseUrlSlash, path, data);

            var rasberryTask = PNO("Measurement/get", new { userId = $"P{id}" });

            await Task.WhenAll(//lightSleepTask, deepSleepTask, remSleepTask, awakeningsTask, caloriesTask, metresAscendedTask, distanceTask, stepsTask, oxygenTask, bodyTemperatureTask, heartRateWatchMinTask, heartRateWatchAvgTask, heartRateWatchMaxTask, heartRateBloodPressureMeterTask, heartRateOximeterTask, weightTask, bloodPressureDiastolicTask, bloodPressureSystolicTask,
                               rasberryTask);

            var rasb = await rasberryTask;

            var ret = new
            {
                pollutionIndex = NonFHIRHelper.FilterAndCompact(rasb, "pollutionIndex"),
                temperatureExternal = NonFHIRHelper.FilterAndCompact(rasb, "temperatureExternal"),
                humidityExternal = NonFHIRHelper.FilterAndCompact(rasb, "humidityExternal"),
                temperature = NonFHIRHelper.FilterAndCompact(rasb, "temperature"),
                humidity = NonFHIRHelper.FilterAndCompact(rasb, "humidity"),
            };

            return ret;
        }


    }
}
