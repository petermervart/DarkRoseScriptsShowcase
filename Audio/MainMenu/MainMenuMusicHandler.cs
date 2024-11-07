using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuMusicHandler : MusicHandler
{
    [SerializeField]
    private List<AudioClip> _mainMenuMusic;

    protected override void Start()
    {
        base.Start();
        PlayMusic();
    }

    public void PlayMusic()
    {
        _audioSource.clip = _mainMenuMusic[Random.Range(0, _mainMenuMusic.Count)];
        _audioSource.Play();
        Invoke("OnClipEnded", _audioSource.clip.length);
    }

    public void OnClipEnded()
    {
        PlayMusic();
    }
}