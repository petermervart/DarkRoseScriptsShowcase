using System.Collections;
using UnityEngine;


public abstract class MusicHandler : MonoBehaviour
{
    [SerializeField] 
    protected OptionsConfig _options;

    [SerializeField] 
    protected AudioSource _audioSource;

    protected virtual void Start()
    {
        _options.OnChangedMusicVolume += OnChangedMusicVolume;
        _audioSource.volume = _options.CurrentMusicVolume;
    }

    public virtual void OnChangedMusicVolume(float newVolume)
    {
        _audioSource.volume = newVolume;
    }

    private void OnDisable()
    {
        _options.OnChangedMusicVolume -= OnChangedMusicVolume;
    }
}
