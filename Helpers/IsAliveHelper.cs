using CSBDashboardServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CSBDashboardServer.Helpers
{
    public static class IsAliveHelper
    {

        public static string IsAlive(string isAlive)
        {
            //staging: curl   http://172.27.0.3:8080/fhir/metadata  or CapabilityStatement
            //staging: curl   -i http://localhost:8081/fhir/metadata or CapabilityStatement
            //test: curl   -i http://localhost:8080/fhir/metadata  or CapabilityStatement
            //staging: curl http://172.19.0.12:5000/api/Status
            //staging:curl -i  localhost:5050/api/Status
            //test: curl -i  localhost:5051/api/Status
            //staging: curl 172.19.0.11:8080/api/patients
            //

            foreach (var item in isAlive.Split(';'))
            {
                var parts = item.Split("|");
                var name = parts[0];
                var url = parts[1];
                try
                {

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Cache-Control", "no-cache");
                        var jsonResponseBody =
                            client.DownloadData(url);
 
                    }
                }
                catch
                {
                    throw new Exception($"{name} is down");
                }
            }
   
            return "OK";
 
    
    }

    }
}

