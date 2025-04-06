using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddMudServices();
        builder.Services.AddBlazoredLocalStorage();

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        ConfigurationServices(builder);

        await builder.Build().RunAsync();
    }

    private static void ConfigurationServices(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddSingleton<BasicConfiguration>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new BasicConfiguration(configuration);
        });

        var config = new BasicConfiguration(builder.Configuration);

        builder!.Services.AddHttpClient(config!.ApiName, client =>
        {
            client.BaseAddress = new Uri(config.ApiUrl);
        });

        builder.Services.AddScoped<UniversalApiManager>();
        builder.Services.AddScoped<UserStorage>(provider =>
        {
            var localStorageService = provider.GetRequiredService<ILocalStorageService>();
            return new UserStorage(localStorageService);
        });
    }
}
