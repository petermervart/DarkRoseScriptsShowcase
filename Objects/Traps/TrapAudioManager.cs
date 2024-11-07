using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAudioManager : AudioHandler
{
    [SerializeField]
    private List<AudioClip> _pipeSwingTrapHit;

    [SerializeField]
    private List<AudioClip> _spikeSwingTrapHit;

    [SerializeField]
    private List<AudioClip> _barbedWireLoop;

    [SerializeField]
    private List<AudioClip> _barricadeDamagedSounds;

    [SerializeField]
    private List<AudioClip> _barricadeDestroyedSounds;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnDamagedBarricade()
    {
        _audioSource.PlayOneShot(_barricadeDamagedSounds[Random.Range(0, _barricadeDamagedSounds.Count)]);
    }

    public void OnBarricadeDestroyed()
    {
        _audioSource.PlayOneShot(_barricadeDestroyedSounds[Random.Range(0, _barricadeDestroyedSounds.Count)]);
    }

    public void OnPipeSwingTrapHit()
    {
        _audioSource.PlayOneShot(_pipeSwingTrapHit[Random.Range(0, _pipeSwingTrapHit.Count)]);
    }

    public void OnSpikeSwingTrapHit()
    {
        _audioSource.PlayOneShot(_spikeSwingTrapHit[Random.Range(0, _spikeSwingTrapHit.Count)]);
    }

    public void OnBarbedWireTrapHandle(bool shouldPlay)
    {
        _audioSource.loop = shouldPlay;
        if (shouldPlay)
        {
            _audioSource.clip = _barbedWireLoop[Random.Range(0, _barbedWireLoop.Count)];
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
        _audioSource.loop = shouldPlay;
    }
}