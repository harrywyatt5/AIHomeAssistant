using System.Diagnostics;
using System.Text;
using Android.Accessibilityservice.AccessibilityService;
using Android.Util;
using IOTProjectApp.WebSocket;

namespace IOTProjectApp;

public class ServiceManager
{
    private static Lazy<ServiceManager> _singleton = new(() => new ServiceManager());
    public static ServiceManager Singleton => _singleton.Value;

    private Task _serviceWorkerChecker;
    private object _workersLock;
    private List<Task> _backgroundWorkers;
    private WebSocketClient? _websocketService;
    private AudioManager _audioManager;
    private CancellationTokenSource _source;

    public ServiceManager()
    {
        _workersLock = new object();
        _backgroundWorkers = new List<Task>();
        _source = new CancellationTokenSource();
        _websocketService = null;
        _audioManager = AudioManager.Singleton;
    }

    public void AddWebSocketAndStart(WebSocketClient client)
    {
        _websocketService = client;
        lock (_workersLock)
        {
            _backgroundWorkers.Add(Task.Run(async () => await _websocketService.Run(_source.Token)));
        }
    }

    public async Task<bool> HasConnected()
    {
        if (_websocketService == null) throw new NullReferenceException();

        while (_websocketService.GetRuntimeState() == RuntimeState.Connecting)
        {
            await Task.Delay(500);
        }

        return _websocketService.GetRuntimeState() == RuntimeState.Connected;
    }

    public void CreateMonitorRuntime(MainPage page)
    {
        _serviceWorkerChecker = Task.Run(async () =>
        {
            while (true)
            {
                // Check none of the background workers have crashed
                lock (_workersLock)
                {
                    foreach (Task task in _backgroundWorkers)
                    {
                        if (!task.IsFaulted) continue;

                        var stackString = task.Exception.InnerException?.StackTrace
                            ?? "[cannot read]";
                        page.UpdateStatus(stackString);
                    }
                }
                await Task.Delay(1000);
            }
        });
    }
}