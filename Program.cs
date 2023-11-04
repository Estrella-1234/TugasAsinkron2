using Microsoft.EntityFrameworkCore;
using TugasAsinkron2.Data;
using TugasAsinkron2.Interfaces;
using TugasAsinkron2.Models.DataContext;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// Tambahkan interface pada program.cs
builder.Services.AddSingleton<IDataAccess, DataAccess>();
builder.Services.AddScoped<IDataFunctions, DataFunctions>();
builder.Services.AddScoped<IExcelHelper, ExcelHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
