using Blazored.SessionStorage;
using CourseEnrollBlaze.Client;
using CourseEnrollBlaze.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting; 

namespace CourseEnrollBlaze.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationState>();
            builder.Services.AddScoped<ISweetAlertService, SweetAlertService>();
            builder.Services.AddAuthorizationCore();
            await builder.Build().RunAsync();
        }
    }
}