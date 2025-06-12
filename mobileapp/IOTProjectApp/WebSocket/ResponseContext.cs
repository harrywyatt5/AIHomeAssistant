using System.Net.WebSockets;
using IOTProjectApp.WebSocket.Message;
using IOTProjectApp.WebSocket.State;

namespace IOTProjectApp.WebSocket;

public class ResponseContext(IState nextState, IReturnMessage returnMessage, WebSocketMessageType type)
{
    public IState NextState { get; private set; } = nextState;
    public IReturnMessage ReturnMessage { get; private set; } = returnMessage;
    public WebSocketMessageType ReturnType { get; private set; } = type;
}