using System;
using System.Net.Http;
using Avalonia.Utilities;
using Microsoft.Extensions.DependencyInjection;
using pmclient.Helpers;
using pmclient.HttpHandlers;
using Refit;

namespace pmclient.Extensions;

public static class RefitClientExtensions
{
   public static IServiceCollection AddWebApiClient<TClient, TMessageHandler>(this IServiceCollection services)
      where TClient : class
      where TMessageHandler : DelegatingHandler
   {
      string baseAddress = ApiEndpoints.BaseAddress;

      services
         .AddRefitClient<TClient>()
         .ConfigureHttpClient(config => config.BaseAddress = new Uri(baseAddress))
         .AddHttpMessageHandler<TMessageHandler>();
      
      return services;
   }
   
   public static IServiceCollection AddWebApiClient<TClient>(this IServiceCollection services)
      where TClient : class
   {
      string baseAddress = ApiEndpoints.BaseAddress;

      services
         .AddRefitClient<TClient>()
         .ConfigureHttpClient(config => config.BaseAddress = new Uri(baseAddress))
         .AddHttpMessageHandler<BearerAuthorizationMessageHandler>();
      
      return services;
   }
}