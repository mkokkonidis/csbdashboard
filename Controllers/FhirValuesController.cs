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
        public async Task<object> Get(int id)
        {
            var auth = Request.Headers.Authorization.ToString();
            var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); // e.g., https://test-retention.biomed.ntua.gr/api/


            if(id == 0)
            {
                id =72;
                apiBaseUrlSlash = "https://staging-retention.biomed.ntua.gr/api/";
                auth = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3eUtxLWdIQ3RFLTF2T1d2Q2otMFVzb3ZnaGY5UG4tOGp1NXZ2SWx1S3JRIn0.eyJleHAiOjE3MjcwMTQ1ODEsImlhdCI6MTcyNzAxNDI4MSwiYXV0aF90aW1lIjoxNzI3MDE0MjgxLCJqdGkiOiIxOTFhNGEwMC01MjQxLTQ0YjMtOWQxNC0yMTZhNjFmYjFhYTAiLCJpc3MiOiJodHRwczovL3N0YWdpbmctcmV0ZW50aW9uLmJpb21lZC5udHVhLmdyL2F1dGgvcmVhbG1zL3JldGVudGlvbiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiI1ODJiNzUzNi1mMGY0LTRkY2UtYjcxNC1jYTZkNzdiYmExN2MiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJkYXNoYm9hcmQiLCJub25jZSI6ImVmNmRiYTM2LTBkN2QtNGFiNy04YzdkLTVjNWUxNGIzMWFhMiIsInNlc3Npb25fc3RhdGUiOiJkMDgxYzJjMC05YTUzLTQ5NDUtYTk0NS0zY2ZlMmJhNWNlMjAiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vc3RhZ2luZy1yZXRlbnRpb24uYmlvbWVkLm50dWEuZ3IiLCJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImNsaW5pY2FsX2Nhc2VfbWFuYWdlciIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXJldGVudGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIG9yZ2FuaXNhdGlvbiBlbWFpbCBwcm9maWxlIiwic2lkIjoiZDA4MWMyYzAtOWE1My00OTQ1LWE5NDUtM2NmZTJiYTVjZTIwIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJEZXYgQWNjb3VudCIsIm9yZ2FuaXNhdGlvbiI6IjE3IiwicHJlZmVycmVkX3VzZXJuYW1lIjoiZGV2IiwiZ2l2ZW5fbmFtZSI6IkRldiIsImZhbWlseV9uYW1lIjoiQWNjb3VudCIsImVtYWlsIjoiZGV2QHRlc3QuY29tIn0.DzB6BNWFy6ENSphCvtqF6f93TBk-vVbO4ID2j0wwG9j_7suzULr9moo5z9PHaXRYs9_-Ip0tkUXhpOpy9TLJund29fTZW20dO8yA4GSlViUY1pe4AZDHpYzP1kDY7rrsCqjcw1mVrV1Y5pyK1fBF2SVrOt3VKSqznhkPeIVzLWiBn4HjdUPJ60m-6UMnWNVXhnw4jHl0wKSJNuWs5oRIo9GGzTtgec7cv90eM2XfZY2_rXjeN3dD7936zc3pQhUymzLIdok8RtdyhBgcfJ2_hnro1PAUAjQCB9LDeU3QcrcWzTqTkbVCV-37TNp8GfExl6ex1DNVjDi2qkhHE5JJXQ";
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
                //pollutionIndex = NonFHIRHelper.FilterAndCompact(rasb, "pollutionIndex"),
                //temperatureExternal = NonFHIRHelper.FilterAndCompact(rasb, "temperatureExternal"),
                //humidityExternal = NonFHIRHelper.FilterAndCompact(rasb, "humidityExternal"),
                //temperature = NonFHIRHelper.FilterAndCompact(rasb, "temperature"),
                //humidity = NonFHIRHelper.FilterAndCompact(rasb, "humidity"),
            };

            return ret;
        }

    }
}
