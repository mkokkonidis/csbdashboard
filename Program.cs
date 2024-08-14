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

// Middleware to remap URLs to index.html for SPA
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Check if the path has no file extension and does not point to an actual file
    if (!Path.HasExtension(path) && !File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        context.Request.Path = "/index.html";
        Console.WriteLine($"{path} => {context.Request.Path}");
    }
    else
    {
        Console.WriteLine($"{path} unchanged as file {Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))} found");
    }
    await next();
});

app.MapControllers();

app.Run();
