using System.Collections.Generic;
using System;

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



// Map /patient-monitoring to serve the index.html file
app.MapGet("/patient-monitoring", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "patient-monitoring", "index.html"));
});

// Map /patient-monitoring to serve the index.html file
app.MapGet("/addPatient", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "addPatient", "index.html"));
});

for (int i = 0; i < 1000; i++)
{
    var path = "builder;patientId=" + i;
    // Map /patient-monitoring to serve the index.html file
    app.MapGet("/"+path, async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, path, "index.html"));
    });
}


//Start server
app.Run();



