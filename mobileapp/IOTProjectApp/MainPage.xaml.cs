using Android;
using Android.Content.PM;
using Android.Util;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using IOTProjectApp.WebSocket;
using Application = Android.App.Application;

namespace IOTProjectApp;

public partial class MainPage : ContentPage
{
    private object _stateLock;
    public MainPage()
    {
        Loaded += OnPageLoaded;
        _stateLock = new object();
        InitializeComponent();
    }

    private async void OnPageLoaded(object? sender, EventArgs args)
    {
        // Ask for permission
        await Permissions.RequestAsync<Permissions.Microphone>();
    }

    public void UpdateStatus(string newText)
    {
        lock (_stateLock)
        {
            StateLabel.Text = newText;
        }
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        // Disable button
        ConnectBtn.IsEnabled = false;
        
        // Start up service manager
        var sm = ServiceManager.Singleton;

        var webClient = new WebSocketClient(new Uri(URIField.Text));
        
        // Register and check has been initialised
        sm.AddWebSocketAndStart(webClient);

        Log.Debug("DEBUG", (await sm.HasConnected()).ToString());
        sm.CreateMonitorRuntime(this);
    }
}