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
    public class ValuesController : ControllerBase
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
                auth = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3eUtxLWdIQ3RFLTF2T1d2Q2otMFVzb3ZnaGY5UG4tOGp1NXZ2SWx1S3JRIn0.eyJleHAiOjE3MjY5NTg3OTQsImlhdCI6MTcyNjk1ODQ5NCwiYXV0aF90aW1lIjoxNzI2OTU3NDQ1LCJqdGkiOiIyNmM0ZThlYy0wMWQ2LTRiZGUtYjgzZi1iZjZiNTMzZDU4NzciLCJpc3MiOiJodHRwczovL3N0YWdpbmctcmV0ZW50aW9uLmJpb21lZC5udHVhLmdyL2F1dGgvcmVhbG1zL3JldGVudGlvbiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiI1ODJiNzUzNi1mMGY0LTRkY2UtYjcxNC1jYTZkNzdiYmExN2MiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJkYXNoYm9hcmQiLCJub25jZSI6IjNmOWM1YmZiLWJiOWMtNGNmYy04ZTEzLTUwNTMyZTdmZWZjMyIsInNlc3Npb25fc3RhdGUiOiI5NThhN2JjYS03ODFlLTQ3M2EtODY0Mi1mYjkwMzI4ZDI0MTYiLCJhY3IiOiIwIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vc3RhZ2luZy1yZXRlbnRpb24uYmlvbWVkLm50dWEuZ3IiLCJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImNsaW5pY2FsX2Nhc2VfbWFuYWdlciIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXJldGVudGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIG9yZ2FuaXNhdGlvbiBlbWFpbCBwcm9maWxlIiwic2lkIjoiOTU4YTdiY2EtNzgxZS00NzNhLTg2NDItZmI5MDMyOGQyNDE2IiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJEZXYgQWNjb3VudCIsIm9yZ2FuaXNhdGlvbiI6IjE3IiwicHJlZmVycmVkX3VzZXJuYW1lIjoiZGV2IiwiZ2l2ZW5fbmFtZSI6IkRldiIsImZhbWlseV9uYW1lIjoiQWNjb3VudCIsImVtYWlsIjoiZGV2QHRlc3QuY29tIn0.XvBZUSdXLAjvT5uFfcsqdfwL-tgHvunBWOpxkLqH_ar3If86429sI_R8XQirI7--GPRtR6scNX2d_-mO2_XuTmbhfUaG9HbCgO4MTjALzBSHRAhPcMtbWo2E1WhADqBl1AiWxsVl1msjhFFN00308QYEVzwUzueUNBiDK0SmMiYZSkd35_PToxuHvnwUp4p1EtuMuoECMGN5gHaMRxe8rC-SKxlWAplcYJbtBG7cgMB3FuKXANg_VD714Sq9pbOt9Cl3qNwGh8nrAxdfngcbRpa1rcR2lAUmzjseASP_Kw1SY6SgL_tb4Zi_U6dK5rRYU1w3zz_zFbY-C-_P3CXzaw";
            }

            Func<string, Task<object>> O = async (spec) => await Task.Run(() => FHIRHelper.CompactFHIRObservations(auth, apiBaseUrlSlash, id, spec));
            Func<string, object, Task<object>> PNO =  (path, data) => NonFHIRHelper.NonFHIRObservations(auth, apiBaseUrlSlash, path, data);


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
            var bloodPressureDiastolicTask = O("code=75367002&category=408746007&component-code=271649006");
            var bloodPressureSystolicTask = O("code=75367002&category=408746007&component-code=271650006");
            var rasberryTask = PNO("Measurement/get", new { userId = $"P{id}" });

            await Task.WhenAll(lightSleepTask, deepSleepTask, remSleepTask, awakeningsTask, caloriesTask, metresAscendedTask, distanceTask, stepsTask, oxygenTask, bodyTemperatureTask, heartRateWatchMinTask, heartRateWatchAvgTask, heartRateWatchMaxTask, heartRateBloodPressureMeterTask, heartRateOximeterTask, weightTask, bloodPressureDiastolicTask, bloodPressureSystolicTask, rasberryTask);

            var rasb = await rasberryTask;

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
                bloodpresureDiastolic = await bloodPressureDiastolicTask,
                bloodpresureSystolic = await bloodPressureSystolicTask,
                rasberry = rasb,
                pollutionIndex = NonFHIRHelper.FilterAndCompact(rasb, "pollutionIndex"),
                temperatureExternal = NonFHIRHelper.FilterAndCompact(rasb, "temperatureExternal"),
                humidityExternal = NonFHIRHelper.FilterAndCompact(rasb, "humidityExternal"),
                temperature = NonFHIRHelper.FilterAndCompact(rasb, "temperature"),
                humidity = NonFHIRHelper.FilterAndCompact(rasb, "humidity"),
            };

            return ret;
        }

        //// GET /<ValuesController>/5
        //[HttpGet("{id}")]
        //public object Get(int id)
        //{
        //    var auth = 
        //        Request.Headers.Authorization.ToString();            
        //    var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); //eg https://test-retention.biomed.ntua.gr/api/

        //    Func<string, object> O = (spec) => CompactObservations(auth, apiBaseUrlSlash, id, spec);


        //    var ret = new {
        //        lightSleep = O("code=762636008&category=29373008"),
        //        deepSleep = O("code=762636008&category=60984000"),
        //        remSleep = O("code=762636008&category=89129007"),
        //        awakenings = O("code=192004002"),
        //        calories = O("code=258790008"),
        //        metresAscended = O("code=310"),
        //        distance = O("code=246132006"),
        //        steps = O("code=309"),
        //        oxygen = O("code=442440005&category=408746007"),
        //        bodyTemperature = O("code=386725007&category=408746007"),
        //        heartRateWatchMin = O("code=364075005&category=62482003"),
        //        heartRateWatchAvg = O("code=364075005&category=255586005"),
        //        heartRateWatchMax = O("code=364075005&category=75540009"),
        //        heartRateBloodPressureMeter = O("code=364075005&category=408746007"),
        //        heartRateOximeter = O("code=364075005&category=59181002"),
        //        weight = O("code=726527001&category=408746007"),
        //        bloodpresureDiastolic = O("code=75367002&category=408746007&component-code=271649006"),
        //        bloodpresureSystolic =  O("code=75367002&category=408746007&component-code=271650006") 
        //    };
        //    return ret;
        //}


    }
}
