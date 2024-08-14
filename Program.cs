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

// Middleware to remap all URLs to index.html
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Check if the file exists in the wwwroot directory
    if (!File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        // If the requested path does not point to a physical file, serve index.html
        context.Request.Path = "/index.html";
    }
    await next();
});

app.MapControllers();

app.Run();
