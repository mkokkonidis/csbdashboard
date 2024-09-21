using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var verbose = (Environment.GetEnvironmentVariable("VERBOSE") ?? "no").ToLower() == "yes";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Home/Error");
}

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

try
{

//    Ping ping = new Ping();
//    PingReply reply = ping.Send("8.8.8.8");  // Google DNS server (IPv4)

//    if (reply.Status == IPStatus.Success)
//    {
//        Console.WriteLine("Ping successful. Network is up.");
//    }
//    else
//    {
//        Console.WriteLine("Ping failed. Check network connectivity.");
//    }


//    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

//    foreach (NetworkInterface adapter in networkInterfaces)
//    {
//        // Get the IP properties of the network interface
//        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

//        // Get the DNS addresses associated with the network interface
//        IPAddressCollection dnsServers = adapterProperties.DnsAddresses;

//        if (dnsServers.Count > 0)
//        {
//            Console.WriteLine($"Network Interface: {adapter.Name}");
//            Console.WriteLine("DNS Servers:");
//            foreach (IPAddress dns in dnsServers)
//            {
//                Console.WriteLine($" - {dns}");
//            }
//            Console.WriteLine();
//        }
//    }
//    var addresses = Dns.GetHostAddresses("google.com"); // Replace with your domain
//    foreach (var address in addresses)
//    {
//        Console.WriteLine($"IP Address: {address}");
//    }
//}
//catch (Exception e)
//{
//    Console.WriteLine($"DNS Resolution Error: {e.Message}");
//}

// Middleware to serve index.html for all non-file requests
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    // Check if the requested path does not match an existing file
    if (!path.StartsWith("/values")
    && !File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        // Serve the index.html file
        if(verbose) Console.WriteLine($"{path} loads index.html");
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }
    else
    {
        if(verbose) Console.WriteLine($"{path} unchanged as file {Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))} found");
        await next(); // Proceed with the next middleware if a file was found
    }





});


















app.MapControllers();

app.Run();
