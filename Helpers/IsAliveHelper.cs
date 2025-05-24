using CSBDashboardServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CSBDashboardServer.Helpers
{
    public static class IsAliveHelper
    {

        public static string IsAlive(string fhirBaseUrlDirect, string nonfhirBaseUrlDirect, string patientManagerBaseUrlDirect)
        {

            try
            {
                //staging: curl   http://172.27.0.3:8080/fhir/metadata  or CapabilityStatement
                //staging: curl   -i http://localhost:8081/fhir/metadata or CapabilityStatement
                //test: curl   -i http://localhost:8080/fhir/metadata  or CapabilityStatement
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Accept", "application/fhir+json");
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Cache-Control", "no-cache");
                    var jsonResponseBody =
                        client.DownloadData(fhirBaseUrlDirect);
                    //var responseBody = (System.Text.Json.JsonElement)
                    //    JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                }
            }
            catch
            {
                throw new Exception($"FHIR is down -- {fhirBaseUrlDirect} --");
            }

            try
            {
                //staging: curl http://172.19.0.12:5000/api/Status
                //staging:curl -i  localhost:5050/api/Status
                //test: curl -i  localhost:5051/api/Status

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Cache-Control", "no-cache");
                    var emptyBody =
                        client.DownloadData(nonfhirBaseUrlDirect);
                }
            }
            catch
            {
                throw new Exception($"Non-FHIR is down; {nonfhirBaseUrlDirect}");
            }

            try
            {
                //staging: curl 172.19.0.11:8080/api/patients
                //
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Cache-Control", "no-cache");
                    var jsonResponseBody =
                        client.DownloadData(patientManagerBaseUrlDirect);
                    //var responseBody = (System.Text.Json.JsonElement)
                    //    JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                }
            }
            catch(Exception exc)
            {
                throw new Exception($"PatientManager is down; {patientManagerBaseUrlDirect} ");
            }



            return "OK";
 
    
    }

    }
}

