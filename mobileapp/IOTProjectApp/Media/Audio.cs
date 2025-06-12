using Android.Media;
using Application = Android.App.Application;
using ArgumentException = System.ArgumentException;
using Uri = Android.Net.Uri;

namespace IOTProjectApp;

public class Audio : IDisposable
{
    public class Builder
    {
        private ReadOnlyMemory<byte> _audioData;
        private string _path;

        private static async Task _saveFileAsync(string path, ReadOnlyMemory<byte> data, CancellationToken cTk)
        {
            await using var newFile = File.Create(path, data.Length);
            await newFile.WriteAsync(data, cTk);
        }

        private static MediaPlayer _createDefaultMedia()
        {
            MediaPlayer newMediaPlayer = new();
            newMediaPlayer.SetAudioAttributes(
                new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Speech)?
                    .SetUsage(AudioUsageKind.Media)?
                    .Build()
            );

            return newMediaPlayer;
        }

        private void _validateSettings()
        {
            // Check audio data or path are not null
            if (_audioData.Length <= 0) throw new ArgumentException("Audio data must be bigger than 0 bytes");
            if (string.IsNullOrEmpty(_path)) throw new ArgumentException("Path must not be null or empty");
        }

        private Uri _getUriFromPath()
        {
            return new Uri
                .Builder()
                .Path(_path)?
                .Build() ?? throw new NullReferenceException("Cannot read file. Do you have permission?");
        }
        

        public Builder()
        {
            _path = Path.GetTempFileName();
            _audioData = new ReadOnlyMemory<byte>();
        }

        public Builder WithAudioData(ReadOnlyMemory<byte> data)
        {
            this._audioData = data;
            return this;
        }

        public Builder WithPath(string filePath)
        {
            this._path = filePath;
            return this;
        }

        public async Task<Audio> BuildAsync(CancellationToken cTk)
        {
            _validateSettings();
            
            // Save file
            await _saveFileAsync(_path, _audioData, cTk);
            
            // Get URI and get media player
            var uri = _getUriFromPath();
            var mediaPlayer = _createDefaultMedia();

            await mediaPlayer.SetDataSourceAsync(Application.Context, uri);
            
            // Start attempting to prepare audio player
            await Task.Run(() => mediaPlayer.Prepare(), cTk);
            
            // Return our new audio player which is ready to run!!!
            return new Audio(uri, mediaPlayer);
        }
    }

    public Uri FilePath { get; private set; }
    private MediaPlayer _mediaPlayer;
    private bool _disposed;

    private Audio(Uri filePath, MediaPlayer player)
    {
        FilePath = filePath;
        _mediaPlayer = player;
        _disposed = false; 
    }

    public void Play() => _mediaPlayer.Start();

    public bool IsPlaying() => _mediaPlayer.IsPlaying;

    public void Stop() => _mediaPlayer.Stop();

    public double GetAudioLength() => (double)_mediaPlayer.Duration / 1000.0D;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_mediaPlayer.IsPlaying) _mediaPlayer.Stop();
            _mediaPlayer.Release();
        }

        _disposed = true;
    }
}