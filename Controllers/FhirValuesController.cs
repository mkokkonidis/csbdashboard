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
    public class FhirValuesController : ControllerBase
    {
        public const bool Verbose = false;
        


        // GET /<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<object> Get(int id, [FromQuery] bool? medicationAdherence)
        {
            var auth = Request.Headers.Authorization.ToString();
            var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); // e.g., https://test-retention.biomed.ntua.gr/api/


            if(id == 0)
            {
                id =72;
                apiBaseUrlSlash = "https://staging-retention.biomed.ntua.gr/api/";
                auth = ""; //"Bearer ..."
            }

            Func<string, Task<object>> O = async (spec) => await Task.Run(() => FHIRHelper.CompactFHIRObservations(auth, apiBaseUrlSlash, id, spec));
            Func<string, object, Task<object>> PNO =  (path, data) => NonFHIRHelper.NonFHIRObservations(auth, apiBaseUrlSlash, path, data);

            if (medicationAdherence == true)
            {
                var medicationAdherenceTask = O("code=418633004");

                return new
                {
                    medicationAdherence = await medicationAdherenceTask
                };
            }
            else
            {
                var lightSleepTask = O("code=762636008&category=29373008");
                var deepSleepTask = O("code=762636008&category=60984000");
                var remSleepTask = O("code=762636008&category=89129007");
                var awakeningsTask = O("code=192004002");
                var caloriesTask = O("code=258790008");
                var metresAscendedTask = O("code=310");
                var distanceTask = O("code=246132006");
                var stepsTask = O("code=309");
                var oxygenTask = O("code=442440005&category=408746007");
                var bodyTemperatureTask = O("code=386725007&category=408746007");
                var heartRateWatchMinTask = O("code=364075005&category=62482003");
                var heartRateWatchAvgTask = O("code=364075005&category=255586005");
                var heartRateWatchMaxTask = O("code=364075005&category=75540009");
                var heartRateBloodPressureMeterTask = O("code=364075005&category=408746007");
                var heartRateOximeterTask = O("code=364075005&category=59181002");
                var weightTask = O("code=726527001&category=408746007");
                var bloodPressureSystolicTask = O("code=75367002&category=408746007&component-code=271649006");
                var bloodPressureDiastolicTask = O("code=75367002&category=408746007&component-code=271650006");

                await Task.WhenAll(lightSleepTask, deepSleepTask, remSleepTask, awakeningsTask, caloriesTask, metresAscendedTask, distanceTask, stepsTask, oxygenTask, bodyTemperatureTask, heartRateWatchMinTask, heartRateWatchAvgTask, heartRateWatchMaxTask, heartRateBloodPressureMeterTask, heartRateOximeterTask, weightTask, bloodPressureDiastolicTask, bloodPressureSystolicTask);//, rasberryTask);


                var ret = new
                {
                    lightSleep = await lightSleepTask,
                    deepSleep = await deepSleepTask,
                    remSleep = await remSleepTask,
                    awakenings = await awakeningsTask,
                    calories = await caloriesTask,
                    metresAscended = await metresAscendedTask,
                    distance = await distanceTask,
                    steps = await stepsTask,
                    oxygen = await oxygenTask,
                    bodyTemperature = await bodyTemperatureTask,
                    heartRateWatchMin = await heartRateWatchMinTask,
                    heartRateWatchAvg = await heartRateWatchAvgTask,
                    heartRateWatchMax = await heartRateWatchMaxTask,
                    heartRateBloodPressureMeter = await heartRateBloodPressureMeterTask,
                    heartRateOximeter = await heartRateOximeterTask,
                    weight = await weightTask,
                    bloodpresureSystolic = await bloodPressureSystolicTask,
                    bloodpresureDiastolic = await bloodPressureDiastolicTask,
             
                };

                return ret;
            }
        }

    }
}
