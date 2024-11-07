using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LevelMusicHandler : MusicHandler
{
    [SerializeField]
    private List<AudioClip> _levelMusicDay;

    [SerializeField]
    private List<AudioClip> _levelMusicNight;

    [SerializeField]
    private List<AudioClip> _nightStartedClip;

    [SerializeField]
    private AudioMixer _audiMixer;

    [SerializeField]
    private float _minMusicVolume;

    [SerializeField]
    private float _maxMusicVolume;

    private bool _isSourceStopped = false;

    private bool _isNight = false;

    protected override void Start()
    {
        base.Start();
        PlayMusic();
    }

    public void NightStarted()
    {
        StartCoroutine(ChangedToNight());
    }

    protected IEnumerator ChangedToNight()
    {
        _audioSource.Stop();

        _isSourceStopped = true;

        AudioClip randomClip = _nightStartedClip[Random.Range(0, _nightStartedClip.Count)];

        _audioSource.PlayOneShot(randomClip);

        yield return new WaitForSeconds(randomClip.length);

        _isNight = true;

        PlayMusic();

        _isSourceStopped = false;
    }

    protected void Update()
    {
        if (!_isSourceStopped && !_audioSource.isPlaying)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        if(_isNight)
            _audioSource.clip = _levelMusicNight[Random.Range(0, _levelMusicNight.Count)];
        else
            _audioSource.clip = _levelMusicDay[Random.Range(0, _levelMusicDay.Count)];
        _audioSource.Play();
    }

    // rescale music volume so it is update even if time is stopped (for the pause menu)
    public override void OnChangedMusicVolume(float newVolume)
    {
        float rescaledVolume = Mathf.Lerp(_minMusicVolume, _maxMusicVolume, newVolume);

        _audiMixer.SetFloat("InGameMusicVolume", rescaledVolume);
    }

    public void OnClipEnded()
    {
        PlayMusic();
    }
}