using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// Middleware to serve index.html for all non-file requests
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Check if the requested path does not match an existing file
    if (!File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        // Serve the index.html file
        Console.WriteLine($"{path} loads index.html");
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }
    else
    {
        Console.WriteLine($"{path} unchanged as file {Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))} found");
        await next(); // Proceed with the next middleware if a file was found
    }





});


















app.MapControllers();

app.Run();
