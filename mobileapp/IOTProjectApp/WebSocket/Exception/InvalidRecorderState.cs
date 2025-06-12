using Exception = Java.Lang.Exception;

namespace IOTProjectApp.WebSocket;

public class InvalidRecorderState : Exception
{
    public InvalidRecorderState(AudioState state, AudioState wantedState) 
        : base($"Could not do this action. Current state is {state} but we needed {wantedState}")
    { }
}