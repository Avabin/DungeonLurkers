﻿using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Identity.Infrastructure;

public static class IdentityHost
{
    private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SourceContext}] [{Level:u4}] {Message:lj}{NewLine} {Exception}";
    public static IHostBuilder CreateDefaultHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .UseSerilog((context, c) =>
                    {
                        var serverUrl = context.Configuration["Seq:Url"];
                        var apiKey    = context.Configuration["Seq:ApiKey"];
                        c.MinimumLevel.Verbose();
                        if (!string.IsNullOrEmpty(serverUrl) && !string.IsNullOrEmpty(apiKey))
                        {
                            c.WriteTo.Seq(serverUrl, apiKey: apiKey).Enrich.FromLogContext().Enrich.WithProperty("Service", "Identity");
                        }
                        c.WriteTo.Console(outputTemplate: OutputTemplate).Enrich.FromLogContext()
                         .WriteTo.File("logs/identity.log", outputTemplate: OutputTemplate).Enrich.FromLogContext();
                    })
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}