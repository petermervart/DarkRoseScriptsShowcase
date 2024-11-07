using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAuidoHandler : AudioHandler
{
    [SerializeField] 
    public float _minPitch = 1.5f;

    [SerializeField] 
    public float _maxPitch = 2.2f;

    [SerializeField]
    private List<AudioClip> gotHurtSounds;

    [SerializeField]
    private List<AudioClip> deathSounds;

    [SerializeField]
    private List<AudioClip> attackSounds;

    [SerializeField]
    private List<AudioClip> gotHitImpactSounds;

    [SerializeField]
    private List<AudioClip> footStepsSoundsWood;

    [SerializeField]
    private List<AudioClip> footStepsSoundsConcrete;

    [SerializeField]
    private List<AudioClip> footStepsSoundsGround;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
    }

    public void OnGotHurt()
    {
        _audioSource.PlayOneShot(gotHurtSounds[Random.Range(0, gotHurtSounds.Count)]);
    }

    public void OnDeath()
    {
        _audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Count)]);
    }

    public void OnAttack()
    {
        _audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Count)]);
    }

    public void OnHitImpact()
    {
        _audioSource.PlayOneShot(gotHitImpactSounds[Random.Range(0, gotHitImpactSounds.Count)]);
    }

    public void OnStep(ESurfaceType surfaceType)
    {
        switch (surfaceType)
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
}