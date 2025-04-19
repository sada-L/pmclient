using System;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using pmclient.Extensions;
using pmclient.Settings;
using ReactiveUI;
using Splat;
using pmclient.HttpHandlers;
using pmclient.RefitClients;
using pmclient.ViewModels;
using pmclient.Views;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace pmclient.Helpers;

public static class Bootstrapper
{
    public static void ConfigureServices()
    {
        var services = new ServiceCollection();
       
        services.AddOptionsWithValidateOnStart<ApiKeySettings>()
             .Configure(settings =>
             {
                 settings.ApiKey = Environment.GetEnvironmentVariable("API_KEY")!;
                 settings.HeaderName = Environment.GetEnvironmentVariable("HEADER_NAME")!;
             });
        
        services.AddTransient<BearerAuthorizationMessageHandler>();
        services.AddTransient<ApiKeyAuthorizationMessageHandler>();
     
        services.AddWebApiClient<IIdentityWebApi, ApiKeyAuthorizationMessageHandler>()
             .AddWebApiClient<IUsersWebApi>();
    
        services.AddSingleton<IScreen,MainWindowViewModel>();
        
        services.UseMicrosoftDependencyResolver();
        
        var serviceProvider = services.BuildServiceProvider();

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        
        Locator.CurrentMutable.RegisterConstant<IServiceProvider>(serviceProvider);
        Locator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginView());
        Locator.CurrentMutable.Register<IViewFor<RegisterViewModel>>(() => new RegisterView());
        Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(() => new HomeView());
        
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }
}