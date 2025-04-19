using Microsoft.Extensions.DependencyInjection;
using pmclient.Views;

namespace pmclient.Extensions;

internal static class ServiceCollectionExtensions
{
   internal static IServiceCollection RegisterViews(this IServiceCollection services,
      ServiceLifetime lifetime = ServiceLifetime.Singleton)
   {
     services.Scan(selector => selector.FromAssemblyOf<MainWindow>()
         .AddClasses()
         .AsSelf()
         .WithLifetime(lifetime));
     
     return services;
   }
}