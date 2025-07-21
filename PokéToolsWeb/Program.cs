using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Pok�ToolsWeb.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<PokemonService>();
builder.Services.AddScoped<TypeService>();
builder.Services.AddScoped<AbilityService>();
builder.Services.AddScoped<MoveService>();
builder.Services.AddScoped<PokemonService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<OffensiveAnalyzer>();
builder.Services.AddScoped<RandomBattleService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
