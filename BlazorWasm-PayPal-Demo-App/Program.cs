using BlazorWasm_PayPal_Demo_App;
using BlazorWasm_PayPal_Demo_App.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PayPalService>();

builder.Services.AddSingleton(provider => builder.Configuration);

await builder.Build().RunAsync();
