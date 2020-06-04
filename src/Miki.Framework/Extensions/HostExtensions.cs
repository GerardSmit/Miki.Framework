using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Framework.Hosting;

namespace Miki.Framework
{
    public static class HostExtensions
    {
        private const string DiscordBotProperty = "MikiBotRegistered";

        public static IHostBuilder ConfigureBot(
            this IHostBuilder hostBuilder,
            Action<IBotApplicationBuilder> configure = null)
        {
            return ConfigureBot(hostBuilder, true, configure);
        }

        internal static IHostBuilder ConfigureBot(
            this IHostBuilder hostBuilder,
            bool throwOnError,
            Action<IBotApplicationBuilder> configure = null)
        {
            hostBuilder.Properties[DiscordBotProperty] = true;
            
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                AddDefaultServices(services);

                services.AddSingleton(p => p.GetRequiredService<IBotApplicationBuilderFactory>().CreateBuilder());

                services.AddBotApplicationBuilderFactory(provider =>
                {
                    var builder = new BotApplicationBuilder
                    {
                        ApplicationServices = provider
                    };
                    
                    if (!hostBuilder.Properties.TryGetValue("UseStartup.StartupType", out var value)
                        || !(value is Type startupType))
                    {
                        startupType = null;
                    }

                    if (configure != null)
                    {
                        configure.Invoke(builder);
                    }
                    else if (!InitializeStartup(provider, startupType, builder))
                    {
                        if (throwOnError)
                        {
                            throw new InvalidOperationException("Could not find the ASP.Net Core startup");
                        }
                    }

                    return builder;
                });
            });

            return hostBuilder;
        }

        internal static bool IsDiscordBotConfigured(this IHostBuilder hostBuilder)
        {
            return hostBuilder.Properties.TryGetValue(DiscordBotProperty, out var value) && Equals(value, true);
        }
        
        private static bool InitializeStartup(IServiceProvider provider, Type startupType, IBotApplicationBuilder builder)
        {
            var startupInterfaceType = Type.GetType("Microsoft.AspNetCore.Hosting.IStartup, Microsoft.AspNetCore.Hosting.Abstractions");
            object startup;

            if (startupInterfaceType != null)
            {
                startup = provider.GetService(startupInterfaceType);

                if (startup != null)
                {
                    startupType = startup.GetType();
                }
            }
            else
            {
                startup = null;
            }

            if (startup == null)
            {
                if (startupType != null)
                {
                    startup = ActivatorUtilities.CreateInstance(provider, startupType);
                }
                else
                {
                    return false;
                }
            }

            var configureMethod = startupType.GetMethod("ConfigureBot");

            if (configureMethod == null)
            {
                return false;
            }
            
            var parameterInfos = configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;

                if (parameterType == typeof(IBotApplicationBuilder))
                {
                    parameters[i] = builder;
                }
                else
                {
                    parameters[i] = provider.GetRequiredService(parameterType);
                }
            }

            configureMethod.Invoke(startup, parameters);

            return true;
        }
        
        internal static void AddDefaultServices(IServiceCollection services)
        {
        }
    }
}