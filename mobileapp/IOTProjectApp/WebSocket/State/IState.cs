namespace IOTProjectApp.WebSocket.State;

public interface IState
{
    public Task<ResponseContext> ReceiveMessageInState(PayloadWrapper message, CancellationToken cancellationToken);
}