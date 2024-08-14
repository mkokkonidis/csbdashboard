using System.Collections.Generic;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //    app.UseExceptionHandler("/Home/Error");
}

// Set default file to index.html
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//Use middleware to remap URLs to index.html
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    Console.WriteLine(path);
    if (!File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        context.Request.Path = "/index.html";
        Console.WriteLine($"{path} => {context.Request.Path}");
    }
    else
        Console.WriteLine($"{path} unchanged as file {Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))} found");
    await next();
});
app.MapControllers();


//Start server
app.Run();



