using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using System.Text;
using CSBDashboardServer.Controllers;

namespace CSBDashboardServer.Helpers
{
    public static class NonFHIRHelper
    {
        static bool Verbose { get { return NonFhirValuesController.Verbose; } }


        public static async Task<dynamic> NonFHIRObservations(string auth, string apiBaseUrlSlash, string endPath, object? data = null)
        {
            const int limit = 3000;
            var infoList = new List<string>();
            var list = new List<object>();

            try
            {
                int page = 0;
                while (true)
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = true
                    };

                    //var client = new HttpClient(handler);

                    // Force IPv4
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.UseNagleAlgorithm = false;
                    System.Net.ServicePointManager.EnableDnsRoundRobin = true;
                    System.Net.ServicePointManager.DnsRefreshTimeout = 0; // Disa
                    using (HttpClient client = new HttpClient(handler))
                    {
                        string url = apiBaseUrlSlash + $"nonfhir/api/{endPath}?limit={limit}&offset={page}&ascending=false";
                        if (Verbose) infoList.Add($"Info: Results from {url} ");

                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        //client.Default.Add("Content-Type", "application/json");
                        client.DefaultRequestHeaders.Add("Authorization", auth);
                        string jsonResponseBody;
                        if (data == null)
                        {
                            jsonResponseBody = await client.GetStringAsync(url);
                        }
                        else
                        {
                            var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
                            response.EnsureSuccessStatusCode();
                            jsonResponseBody = await response.Content.ReadAsStringAsync();
                        }



                        var responseBody = (System.Text.Json.JsonElement)
                            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);

                        var measurementEntries = responseBody.GetProperty("measurementEntries").EnumerateArray();
                        int count = 0;
                        foreach (var entry in measurementEntries)
                        {
                            /*{
          "createdDateTime": "2024-09-20T20:00:06.821604",
          "deviceId": "rpi",
          "deviceImei": "e4:5f:01:e1:8b:08",
          "measurementDatetime": "2024-09-20T20:00:06",
          "measurementId": 14595,
          "metadata": null,
          "modifiedDateTime": "2024-09-20T20:00:06.821601",
          "state": "1",
          "type": "pollutionIndex",
          "userId": "P72"
        },*/
                            list.Add(new
                            {
                                measurementDatetime = entry.GetString("measurementDatetime"),
                                state = entry.GetString("state"),
                                type = entry.GetString("type")
                            });
                            //list.Add(entry);
                            count++;
                        }
                        if (count < limit) break;

                        page++;


                    }

                }
                return new
                {
                    results = new { measurementEntries = list },

                    debug = infoList
                };


            }
            catch (Exception ex)
            {
                infoList.Add($"Info: {ex.Message}");
                return new
                {
                    //results = new { },
                    debug = infoList
                };
            }

        }


        public static object FilterAndCompact(dynamic returnedObject, string type)
        {
            try
            {
                List<dynamic> bigList = returnedObject.results.measurementEntries;
                var list =
                    bigList.Where(_ => _.type == type &&
                             Convert.ToInt32(_.measurementDatetime.Split('T')[1].Split(':')[0]) % 8 == 0)
                 .Select(_ => new decimal[] { JSDateHelper.ToJSTicks(_.measurementDatetime), Convert.ToDecimal(_.state) })
                .ToList();

                //Sort
                list.Sort((a, b) => a[0].CompareTo(b[0]));

                //Ensure retList is a function from time to values
                list = list
                    .GroupBy(pair => pair[0])  // Group by the first element
                    .Select(group => group.First()) // Select the first pair in each group
                    .ToList();

                return new
                {
                    results = list,
                    debug = returnedObject.debug
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    //results = new { },
                    debug = returnedObject.debug
                };

            }
        }
    }
}

