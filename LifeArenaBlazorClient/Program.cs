using Avangardum.LifeArena.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LifeArenaBlazorClient;
using LifeArenaBlazorClient.Interfaces;
using LifeArenaBlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<HttpClient>(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<ILivingCellsArrayPreserializer, LivingCellsArrayPreserializer>();

await builder.Build().RunAsync();