﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Tryouts.Messaging.Client;
using MorganStanley.ComposeUI.Tryouts.Messaging.Client.Transport.WebSocket;
using MorganStanley.ComposeUI.Tryouts.Messaging.Server.Transport.WebSocket;

namespace WpfHost;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureServices(
                (context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddMessageRouterServer(mr => mr.UseWebSockets(ws => ws.RootPath = "/ws"));
                })
            .ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Debug).AddProvider(new MainWindowLoggerProvider(this)));
            

        Host = hostBuilder.Build();
        Logger = Host.Services.GetRequiredService<ILogger<App>>();
    }

    public new MainWindow? MainWindow
    {
        get => (MainWindow?)base.MainWindow;
        set => base.MainWindow = value;
    }

    internal IHost Host { get; private set; }
    internal ILogger<App> Logger { get; }

    internal async void OnMainWindowClosing(object sender, CancelEventArgs args)
    {
        if (_shutdownCompleted)
            return;

        if (_isClosing)
        {
            args.Cancel = true;

            return;
        }

        _isClosing = true;
        args.Cancel = true;
        Logger.LogInformation("Main window closed, shutting down");
        await OnShutdown();
        _shutdownCompleted = true;
        Logger.LogDebug("Application shutdown complete, closing main window");
        //this.MainWindow?.Close();
    }

    private bool _isClosing;
    private bool _shutdownCompleted;

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await Host.StartAsync();

        MainWindow = Host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        var server = Host.Services.GetRequiredService<IMessageRouterWebSocketServer>();
        Logger.LogInformation("Server listening on port {port}", server.Port);

        var messageRouter = MessageRouter.Create(
            mr => mr.UseWebSocket(new MessageRouterWebSocketOptions { Uri = new Uri($"ws://localhost:{server.Port}{server.RootPath}") }));

        await messageRouter.ConnectAsync().ConfigureAwait(false);
        Logger.LogInformation("Message Router client connected");
        await messageRouter.PublishAsync("ApplicationStarted").ConfigureAwait(false);
        Logger.LogInformation("Message Router publish successful");
    }

    private async Task OnShutdown()
    {
        await Host.StopAsync();
    }

    //internal class WebHostStartup
    //{
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddMessageRouterServer();
    //    }

    //    public void Configure(IApplicationBuilder app)
    //    {
    //        app.UseWebSockets();
    //        app.MapMessageRouterWebSocketEndpoint("/ws");
    //    }
    //}
}
