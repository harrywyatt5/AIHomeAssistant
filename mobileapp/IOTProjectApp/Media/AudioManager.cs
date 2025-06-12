namespace IOTProjectApp;

public class AudioManager
{
    private const int TimeUntilIter = 5000;
    private static readonly Lazy<AudioManager> _singleton = new(() => new AudioManager());
    public static AudioManager Singleton => _singleton.Value;

    private volatile List<Audio> _registeredAudios;
    private object _lock;

    private AudioManager()
    {
        _registeredAudios = new List<Audio>();
        _lock = new object();
    }

    public void AddAudio(Audio audio)
    {
        lock (_lock)
        {
            _registeredAudios.Add(audio);
        }
    }

    public async Task RunAudioManager(CancellationToken cTk)
    {
        // Audio manager iterates through audio objects and checks if they can be destroyed
        while (true)
        {
            lock (_lock)
            {
                // Copy across to temp array
                var iterArray = _registeredAudios.ToArray();
                foreach (Audio audio in iterArray)
                {
                    if (!audio.IsPlaying())
                    {
                        _registeredAudios.Remove(audio);
                        audio.Dispose();
                    }
                }
            }
            
            // Await next iteration
            await Task.Delay(TimeUntilIter, cTk);
        }
    }
}