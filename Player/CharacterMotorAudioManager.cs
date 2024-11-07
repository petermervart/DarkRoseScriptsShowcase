using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterAudioManager : AudioHandler
{
    public enum ELoopedSounds
    {
        Looting = 0,

        Crafting = 1,

        Nothing = 1000
    }

    [SerializeField] 
    private List<AudioClip> hitGroundSounds;

    [SerializeField] 
    private List<AudioClip> footStepsSoundsGround;

    [SerializeField] 
    private List<AudioClip> footStepsSoundsConcrete;

    [SerializeField] 
    private List<AudioClip> footStepsSoundsWood;

    [SerializeField]
    private List<AudioClip> craftingSounds;

    [SerializeField]
    private List<AudioClip> placedBuildingSounds;

    [SerializeField]
    private List<AudioClip> gotHurtSounds;

    [SerializeField]
    private List<AudioClip> hitSomethingSounds;

    [SerializeField]
    private List<AudioClip> hitNothingSound;

    [SerializeField]
    private List<AudioClip> gotHealedSounds;

    [SerializeField]
    private List<AudioClip> lootingSounds;

    [SerializeField]
    private List<AudioClip> flashlightSounds;

    private ELoopedSounds _currentLoopedSound = ELoopedSounds.Nothing;

    public override void OnChangedPausedAudio(bool isPaused)
    {
        if(_currentLoopedSound == ELoopedSounds.Crafting)
        {
            if(isPaused)
                _audioSource.Stop();

            return;
        }

        base.OnChangedPausedAudio(isPaused);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnHitGround(Vector3 locationRB)
    {
        _audioSource.PlayOneShot(hitGroundSounds[Random.Range(0, hitGroundSounds.Count)]);
    }

    public void OnHitSomething()
    {
        _audioSource.PlayOneShot(hitSomethingSounds[Random.Range(0, hitSomethingSounds.Count)]);
    }

    public void OnHitNothing()
    {
        _audioSource.PlayOneShot(hitNothingSound[Random.Range(0, hitNothingSound.Count)]);
    }

    public void OnGotHurt()
    {
        _audioSource.PlayOneShot(gotHurtSounds[Random.Range(0, gotHurtSounds.Count)]);
    }

    public void OnPlacedBuilding()
    {
        _audioSource.PlayOneShot(placedBuildingSounds[Random.Range(0, placedBuildingSounds.Count)]);
    }

    public void OnHealed()
    {
        _audioSource.PlayOneShot(gotHealedSounds[Random.Range(0, gotHealedSounds.Count)]);
    }

    public void OnFlashlight()
    {
        _audioSource.PlayOneShot(flashlightSounds[Random.Range(0, flashlightSounds.Count)]);
    }

    public void OnFootSteps(Vector3 locationRB, float velocity, ESurfaceType groundMaterial)
    {
        switch (groundMaterial)
        {
            case ESurfaceType.Concrete:
                _audioSource.PlayOneShot(footStepsSoundsConcrete[Random.Range(0, footStepsSoundsConcrete.Count)]);
                break;
            case ESurfaceType.Wood:
                _audioSource.PlayOneShot(footStepsSoundsWood[Random.Range(0, footStepsSoundsWood.Count)]);
                break;
            default:
                _audioSource.PlayOneShot(footStepsSoundsGround[Random.Range(0, footStepsSoundsGround.Count)]);
                break;
        }
    }

    public void OnLooting(bool shouldPlay)
    {
        _audioSource.loop = shouldPlay;
        if (shouldPlay)
        {
            _audioSource.clip = lootingSounds[Random.Range(0, lootingSounds.Count)];
            _audioSource.Play();
            _currentLoopedSound = ELoopedSounds.Looting;
        }
        else
        {
            _audioSource.clip = null;
            _audioSource.Stop();
        }
    }

    public void OnCrafting(bool shouldPlay)
    {
        _audioSource.loop = shouldPlay;
        if (shouldPlay)
        {
            _audioSource.clip = craftingSounds[Random.Range(0, craftingSounds.Count)];
            _audioSource.Play();
            _currentLoopedSound = ELoopedSounds.Crafting;
        }
        else
        {
            _audioSource.Stop();
        }
        _audioSource.loop = shouldPlay;
    }
}
