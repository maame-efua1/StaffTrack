
using StaffTrackUI.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StaffTrackUI;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register HttpClient with the correct base address
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7229/api/") // Ensure this matches your API endpoint
});

// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LoginService>();



await builder.Build().RunAsync();
