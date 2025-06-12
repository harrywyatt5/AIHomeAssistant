using System.Text.Json;
using System.Text.Json.Serialization;
using Android.Util;
using IOTProjectApp.WebSocket.State;
using Java.Util;

namespace IOTProjectApp.WebSocket;

using System.Net.WebSockets;

public class WebSocketClient
{
    private readonly ClientWebSocket _internal;
    private readonly Uri _uri;
    private readonly JsonSerializerOptions _jsonOptions;
    private SemaphoreSlim _stateObjLock;
    private volatile IState _currentState;

    private object _stateLock;
    private volatile RuntimeState _runtimeState;

    public WebSocketClient(Uri uri)
    {
        _internal = new();
        _uri = uri;
        _jsonOptions = JsonOptionsProvider.Singleton.GetGlobalOptions();
        _stateObjLock = new SemaphoreSlim(1);
        _currentState = new IdleState();
        _stateLock = new object();
        lock (_stateLock)
        {
            _runtimeState = RuntimeState.Connecting;
        }
    }

    private async Task _connect(CancellationToken cancelToken)
    {
        Log.Debug("DEBUG", "Attempting to connect");
        await _internal.ConnectAsync(_uri, cancelToken);
        
        // Delay the return of this function until it has connected or failed
        while (_internal.State == WebSocketState.Connecting)
        {
            Log.Debug("DEBUG", "Waiting to connect");
            await Task.Delay(100, cancelToken);
        }
        
        // When we connect
        SetRuntimeState(RuntimeState.Connected);
    }
    private async Task _closeSocket(WebSocketClosure closure, CancellationToken cancelToken)
    {
        await _internal.CloseAsync(closure.Status, closure.CloseReason, cancelToken);
        SetRuntimeState(RuntimeState.Failed);
    }

    private async Task _handleReceive(CancellationToken cancelToken)
    {
        using var memoryStream = new MemoryStream();
        var tempBuffer = new ArraySegment<byte>(new byte[8192]);
        WebSocketReceiveResult result;
        
        // Put all in a buffer
        do
        {
            result = await _internal.ReceiveAsync(tempBuffer, cancelToken);
            Log.Debug("DEBUG", "Received data");
            if (tempBuffer.Array is not null) {
                memoryStream.Write(tempBuffer.Array, tempBuffer.Offset, result.Count);
            }
        } while (!result.EndOfMessage);
        
        // Reset seek
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        Log.Debug("DEBUG", "Received full frame");
        if (result.MessageType == WebSocketMessageType.Close) throw new ServerClosureException();
        
        // Only process the state on one thread at any time
        await _stateObjLock.WaitAsync(cancelToken);
        try
        {
            var handleContext = await _currentState.ReceiveMessageInState(
                new PayloadWrapper(memoryStream, result.MessageType),
                cancelToken
            );

            // Update state
            _currentState = handleContext.NextState;
            
            // Serialize message
            var responseBytes = await handleContext.ReturnMessage.GetMessageAsBytes();

            Log.Debug("DEBUG", $"Sending {responseBytes.Length} bytes for state {_currentState.GetType()}");
            await _internal.SendAsync(
                new ReadOnlyMemory<byte>(responseBytes),
                handleContext.ReturnType,
                WebSocketMessageFlags.EndOfMessage,
                cancelToken
            );
        }
        finally
        {
            _stateObjLock.Release();
        }
    }

    public async Task Run(CancellationToken cToken)
    {
        // We can use the websocket state to see if we connected correctly
        await _connect(cToken);

        if (_internal.State != WebSocketState.Open)
        {
            SetRuntimeState(RuntimeState.CouldNotConnect);
            throw new WebSocketConnectionException("Could not connect. Reason: " + (_internal.CloseStatusDescription ?? "Not provided"));
        }

        
        while (true)
        {
            await _handleReceive(cToken);
        }
    }

    public RuntimeState GetRuntimeState()
    {
        lock (_stateLock)
        {
            return _runtimeState;
        }
    }

    private void SetRuntimeState(RuntimeState state)
    {
        lock (_stateLock)
        {
            _runtimeState = state;
        }
    }
}