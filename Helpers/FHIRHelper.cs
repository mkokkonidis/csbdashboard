using CSBDashboardServer.Controllers;
using System.Net;
using System.Text.Json;

namespace CSBDashboardServer.Helpers
{
    public static class FHIRHelper
    {
        static bool Verbose { get { return ValuesController.Verbose; } }


        public static dynamic CompactFHIRObservations(string auth, string apiBaseUrlSlash, int patient, string spec)
        {
            string? componentCode =
                spec
                    .Split('&')
                    .Where(_ => _.StartsWith("component-code="))
                    .Select(_ => _.Split('=')[1])
                    .FirstOrDefault();

            const int pageSize = 1000;
            var retList = new List<decimal[]>();
            var infoList = new List<string>();

            string url = $"{apiBaseUrlSlash}fhir/Observation?patient=P{patient}&{spec}";
            if (Verbose) infoList.Add($"Info: Results from {url} " + (componentCode != null ? $" with component-code={componentCode}" : ""));
            try
            {
                while (true)
                {

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", auth);
                        var jsonResponseBody =
                            client.DownloadData(url + $"&_getpagesoffset={retList.Count()}&_count={pageSize}");
                        var responseBody = (System.Text.Json.JsonElement)
                            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                        if (Verbose) infoList.Add($"Info: deserialised");
                        var list = responseBody.GetProperty("entry").EnumerateArray();
                        if (Verbose) infoList.Add($"Info: entry array found");
                        foreach (var item in list)
                        {
                            var resource = item.GetProperty("resource");
                            var effectiveDateTime = resource.GetProperty("effectiveDateTime").GetString();
                            var value =
                                componentCode == null ?
                                    resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal() :
                                    resource.GetProperty("component")
                                    .EnumerateArray().ToList()
                                            .Where(_ =>
                                                   _.GetProperty("code")
                                                    .GetProperty("coding")[0]
                                                    .GetProperty("code").GetString() == componentCode)
                                            .FirstOrDefault()
                                            .GetProperty("valueQuantity").GetProperty("value").GetDecimal();


                            retList.Add(new decimal[] {
                                JSDateHelper.ToJSTicks(effectiveDateTime),
                                value });
                        }
                        if (list.Count() < pageSize) break;
                    }

                }
            }
            catch (Exception ex)
            {
                infoList.Add($"Info: {ex.Message}");
            }

            //Sort
            retList.Sort((a, b) => a[0].CompareTo(b[0]));

            //Ensure retList is a function from time to values
            retList = retList
                .GroupBy(pair => pair[0])  // Group by the first element
                .Select(group => group.First()) // Select the first pair in each group
                .ToList();

            return new
            {
                results = retList,
                debug = infoList
            };
        }

    }
}

