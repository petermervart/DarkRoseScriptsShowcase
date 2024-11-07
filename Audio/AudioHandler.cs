using System.Collections;
using UnityEngine;


public class AudioHandler : MonoBehaviour
{
    [SerializeField]
    protected OptionsConfig _options;

    [SerializeField] 
    protected AudioSource _audioSource;

    protected virtual void Start()
    {
        _options.OnChangedGameplayVolume += OnChangedGameplayVolume;
        if(UIManager.Instance != null)
            UIManager.Instance.OnChangedPauseGameState += OnChangedPausedAudio;
        _audioSource.volume = _options.CurrentGameplayVolume;
    }

    public virtual void OnChangedPausedAudio(bool isPaused)
    {
        if (isPaused)
        {
            _audioSource.Pause();
        }
        else if(_audioSource.clip != null)
        {
            _audioSource.Play();
        }
    }

    public virtual void OnChangedGameplayVolume(float newVolume)
    {
        _audioSource.volume = newVolume;
    }

    private void OnDisable()
    {
        _options.OnChangedGameplayVolume -= OnChangedGameplayVolume;
        UIManager.Instance.OnChangedPauseGameState -= OnChangedPausedAudio;
    }
}
