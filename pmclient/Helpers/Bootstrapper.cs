using System;
using System.Reflection;
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
        
        
       //services.AddDbContext<PostgresContext>();
       //
       //services.AddScoped<IUserRepository, UserRepository>();
       //services.AddScoped<ICardRepository, CardRepository>();
       //services.AddScoped<IGroupRepository, GroupRepository>();
       //services.AddScoped<IUserCardGroupRepository, UserCardGroupRepository>();
       //
      // services.AddSingleton<IUserService, UserService>();
       //services.AddSingleton<ICardService, CardService>();
       //services.AddSingleton<IGroupService, GroupService>();
       //services.AddSingleton<IUserCardGroupService, UserCardGroupService>();
       //
       //services.AddSingleton<IServiceManager, ServiceManager>();
       //services.AddSingleton<UserDataService>();
       //services.AddSingleton<TwoFactorAuthService>();
       //
       //services.AddSingleton<MainWindowViewModel>();
       //services.AddSingleton<IScreen, MainViewModel>();                           
        //services.AddSingleton<MainViewModel>();
        //services.AddSingleton<UserViewModel>();
        //services.AddTransient<HomeViewModel>();
        //services.AddTransient<RegisterViewModel>();
        //services.AddTransient<LoginViewModel>();
        
        /*services.AddTransient<SingleViewModel>();
        services.AddTransient<SingleMainViewModel>();
        services.AddTransient<SingleLoginViewModel>();*/
        
        //services.UseMicrosoftDependencyResolver();
        
        var serviceProvider = services.BuildServiceProvider();

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        
        Locator.CurrentMutable.RegisterConstant(serviceProvider, typeof(IServiceProvider));
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());
        Locator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginView());
        Locator.CurrentMutable.Register<IViewFor<RegisterViewModel>>(() => new RegisterView());
        /*Locator.CurrentMutable.Register<IViewFor<UserViewModel>>(() => new UserView());
        Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(() => new HomeView());#1#*/
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }
}