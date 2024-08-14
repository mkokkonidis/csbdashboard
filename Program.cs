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

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!File.Exists(Path.Combine(app.Environment.WebRootPath, path.TrimStart('/'))))
    {
        context.Request.Path = "/index.html";
    }
    await next();
});
app.MapControllers();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//Action<string> mapToIndexHTML = (path) =>
//{
//    app.MapGet(path, async context =>
//    {
//        context.Response.ContentType = "text/html";
//        await context.Response.SendFileAsync("wwwroot/index.html");
//    });
//};

////Static mapping
//mapToIndexHTML("/patients-monitoring");
//mapToIndexHTML("/addPatient");
//for (int i = 0; i < 1000; i++)
//    mapToIndexHTML("/builder;patientId=" + i);


//Start server
app.Run();



