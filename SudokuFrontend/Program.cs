using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using SudokuFrontend.AuthProviders;
using SudokuFrontend.HttpRepository;
using System.Configuration;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var webApiAddress = builder.Configuration.GetValue(typeof(string), "WebApiAddress");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri(webApiAddress.ToString())
}.EnableIntercept(sp));
builder.Services.AddHttpClientInterceptor();
//builder.Services.AddScoped<AuthenticationStateProvider, TestAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IHighscoresHttpRepository, HighscoresHttpRepository>();
builder.Services.AddScoped<ISudokuHttpRepository, SudokuHttpRepository>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<HttpInterceptorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
