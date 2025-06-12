using Android.Media;
using Android.Util;
using AndroidX.Lifecycle;
using IOTProjectApp.WebSocket;
using Application = Android.App.Application;

namespace IOTProjectApp;

public class Recorder : IDisposable
{
    public string FilePath { get; private set; }
    public AudioState State { get; private set; }
    public Android.Media.OutputFormat OutputFormat { get; private set; }
    public Android.Media.AudioEncoder Encoder { get; private set; }
    public Android.Media.AudioSource Source { get; private set; }
    
    private MediaRecorder _internal;
    private bool _isDisposed;

    public Recorder()
    {
        // Create a file
        FilePath = Path.GetTempFileName();
        OutputFormat = OutputFormat.Ogg;
        Encoder = AudioEncoder.Opus;
        Source = AudioSource.Mic;
        State = AudioState.NotPrepared;

        _internal = new MediaRecorder(Application.Context);
        _internal.SetAudioSource(Source);
        _internal.SetOutputFormat(OutputFormat);
        _internal.SetOutputFile(FilePath);
        _internal.SetAudioEncoder(Encoder);
    }
    
    // Sorry, prepare is blocking :/
    public void Prepare(CancellationToken cTk)
    {
        if (State == AudioState.NotPrepared)
        {
            State = AudioState.Preparing;
            Log.Debug("DEBUG", "ABout to prepare");
            _internal.Prepare();
            State = AudioState.Prepared;
        }
        else
        {
            Log.Debug("DEBUG", $"Could not prepare, currently in state {State}");
        }
    }

    public void StartRecording()
    {
        if (State == AudioState.Prepared)
        {
            State = AudioState.Recording;
            _internal.Start();
        }
        else
        {
            Log.Error("ERROR", $"Cannot start in state {State}");
        }
    }

    public void StopRecording()
    {
        if (State == AudioState.Recording)
        {
            _internal.Stop();
            State = AudioState.Stopped;
        }
        else
        {
            Log.Error("Error", $"Cannot stop in state {State}");
        }


    }

    public async Task<byte[]> ReadRecordingAsync(CancellationToken cTk)
    {
        if (State == AudioState.Recording)
        {
            Log.Warn("WARN", "Stopping recording as we have had a request to read");
            this.StopRecording();
        }

        if (State == AudioState.Stopped)
        {
            // read bytes from our file and return
            return await File.ReadAllBytesAsync(FilePath.ToString(), cTk);
        }
        
        throw new InvalidRecorderState(State, AudioState.Stopped);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            if (State == AudioState.Recording) this.StopRecording();
            _internal.Release();
            _isDisposed = true;
        }
    }
}