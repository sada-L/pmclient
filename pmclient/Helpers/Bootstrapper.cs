using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using pmclient.Extensions;
using pmclient.HttpHandlers;
using pmclient.RefitClients;
using pmclient.Services;
using pmclient.Settings;
using pmclient.ViewModels;
using pmclient.Views;
using ReactiveUI;
using Splat;
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
                settings.ApiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "X-API-KEY";
                settings.HeaderName = Environment.GetEnvironmentVariable("HEADER_NAME") ?? "X-HEADER-NAME";
            });

        services.AddTransient<BearerAuthorizationMessageHandler>();
        services.AddTransient<ApiKeyAuthorizationMessageHandler>();

        services.AddWebApiClient<IIdentityWebApi, ApiKeyAuthorizationMessageHandler>()
            .AddWebApiClient<IUsersWebApi>()
            .AddWebApiClient<ICardWebApi>()
            .AddWebApiClient<IGroupWebApi>()
            .AddWebApiClient<ITwoFaWebApi>();

        services.AddSingleton<AuthService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<CardService>();
        services.AddSingleton<GroupService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<TwoFaService>();

        services.AddSingleton<IScreen, MainViewModel>();

        services.UseMicrosoftDependencyResolver();

        var serviceProvider = services.BuildServiceProvider();

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();

        Locator.CurrentMutable.RegisterConstant<IServiceProvider>(serviceProvider);
        Locator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginView());
        Locator.CurrentMutable.Register<IViewFor<RegisterViewModel>>(() => new RegisterView());
        Locator.CurrentMutable.Register<IViewFor<CardViewModel>>(() => new CardView());
        Locator.CurrentMutable.Register<IViewFor<GroupViewModel>>(() => new GroupView());
        Locator.CurrentMutable.Register<IViewFor<HeaderGroupViewModel>>(() => new HeaderGroupView());
        Locator.CurrentMutable.Register<IViewFor<AuthViewModel>>(() => new AuthView());

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(() => new HomeView());
        }
        else
        {
            Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(() => new HomeMobileView());
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());
            Locator.CurrentMutable.Register<IViewFor<PinViewModel>>(() => new PinView());
        }

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }
}