using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterBehaviour : MonoBehaviour
{
    [Header("Health")]

    [SerializeField]
    private float _maxHealth = 300f;

    public UnityEvent<float> OnHealthChanged = new UnityEvent<float>();

    [Header("Healing")]

    [SerializeField]
    private float _timeBetweenUsingMedkit = 2f;

    [SerializeField]
    private ConsumableConfig _medkitConsumableItem;

    [SerializeField] 
    UnityEvent<float, bool> OnMedkitLoadingTimeChanged = new UnityEvent<float, bool>();

    [SerializeField] 
    UnityEvent<bool> OnAttackChanged = new UnityEvent<bool>();

    [Header("Attacking")]

    [SerializeField] 
    private float _minDistanceToHitAttack = 2f;

    [SerializeField] 
    private LayerMask _attackLayerMask = ~0;

    [SerializeField] 
    private float _minDamage = 20;

    [SerializeField]
    private float _maxDamage = 40;

    [SerializeField]
    private float _destructableMinDamage = 20;

    [SerializeField]
    private float _destructableMaxDamage = 40;

    [SerializeField] 
    private float _attackMaxAngle = 45f;

    [SerializeField] 
    private Transform _playerCamera;

    [SerializeField]
    private ParticleSystem _bloodParticle;

    [SerializeField]
    private Vector2 _bloodParticleRandomXOffset;

    [SerializeField]
    private Vector2 _bloodParticleRandomYOffset;

    [SerializeField]
    private Vector2 _bloodParticleRandomZOffset;

    [Header("Flashlight")]

    [SerializeField]
    private GameObject _light;

    [Header("Other")]

    public UnityEvent<bool> OnPauseMenuChanged = new UnityEvent<bool>();

    public UnityEvent OnCharacterDied = new UnityEvent();

    public UnityEvent OnCharacterGotHurt = new UnityEvent();

    public UnityEvent OnCharacterGotHealed = new UnityEvent();

    public UnityEvent OnHitSomething = new UnityEvent();

    public UnityEvent OnDidNotHitAnything = new UnityEvent();

    public UnityEvent OnFlashLightChanged = new UnityEvent();

    private float _currentHealth;
    private bool _isHealingLocked = false;

    private bool _isAttackLocked = false;
    private bool _isAttacking = false;

    private bool _isPauseMenuOpened = false;

    private bool _isLightOn = false;

    protected void OnHeal(InputValue value)
    {
        if (value.isPressed)
        {
            if (!_isHealingLocked && InventoryManager.Instance.CheckItemAvailability(_medkitConsumableItem.InventoryItem) && _currentHealth != _maxHealth)
            {
                OnHealed(_medkitConsumableItem.HealAmount);

                InventoryManager.Instance.RemoveItemFromInventory(_medkitConsumableItem.InventoryItem, 1);
            }
        }
    }

    protected void OnAttack(InputValue value)
    {
        if (value.isPressed && !_isAttacking && !_isAttackLocked)
        {
            OnStartAttack();
        }
        else if (!value.isPressed && _isAttacking)
        {
            OnEndAttack();
        }
    }

    public void OnPauseMenu()
    {
        _isPauseMenuOpened = !_isPauseMenuOpened;
        OnPauseMenuChanged.Invoke(_isPauseMenuOpened);
    }

    protected void OnFlashLight(InputValue value)
    {
        _isLightOn = !_isLightOn;
        _light.SetActive(_isLightOn);
        OnFlashLightChanged.Invoke();
    }

    // same attack check as enemy but also checks for barricade, so player can destroy them
    protected void CheckForAttackHit()
    {
        float radians = _attackMaxAngle * Mathf.Deg2Rad;

        Vector3 forwardDirection = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _minDistanceToHitAttack, forwardDirection, 0f, _attackLayerMask);

        bool hitSomething = false;

        bool hitDamageable = false;

        foreach (RaycastHit hit in hits)
        {
            Vector3 directionToTarget = (hit.collider.bounds.center - _playerCamera.transform.position).normalized;

            float dotProduct = Vector3.Dot(forwardDirection, directionToTarget);

            if (dotProduct >= Mathf.Cos(radians))
            {
                if(!hitSomething)
                    hitSomething = true;

                if (hit.collider.TryGetComponent<Enemy>(out var enemyScript))
                {
                    if (!hitDamageable)
                        hitDamageable = true;

                    enemyScript.OnEnemyDamaged(Random.Range(_minDamage, _maxDamage));

                    ParticleSystem bloodParticle = Instantiate(_bloodParticle);
                    bloodParticle.transform.position = hit.collider.bounds.center + new Vector3(Random.Range(_bloodParticleRandomXOffset.x, _bloodParticleRandomXOffset.y), Random.Range(_bloodParticleRandomYOffset.x, _bloodParticleRandomYOffset.y), Random.Range(_bloodParticleRandomZOffset.x, _bloodParticleRandomZOffset.y));
                    bloodParticle.transform.forward = hit.normal;
                }
                else if(hit.collider.TryGetComponent<DestructableObstruction>(out var destructableScript))
                {
                    if (!hitDamageable)
                        hitDamageable = true;

                    destructableScript.OnTakeDamage(Random.Range(_destructableMinDamage, _destructableMaxDamage));
                }
            }
        }

        if (hitSomething)
            OnHitSomething.Invoke();

        if(!hitSomething)
            OnDidNotHitAnything.Invoke();
    }

    public void OnCheckAttack()
    {
        CheckForAttackHit();
    }

    protected void OnStartAttack()
    {
        OnAttackChanged.Invoke(true);
        _isAttacking = true;
    }

    protected void OnEndAttack()
    {
        OnAttackChanged.Invoke(false);
        _isAttacking = false;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    // Handle medkit loading after using a medkit
    private IEnumerator UpdateMedkitLoading()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _timeBetweenUsingMedkit)
        {
            elapsedTime += Time.deltaTime;

            OnMedkitLoadingTimeChanged.Invoke(elapsedTime/_timeBetweenUsingMedkit, true);
            yield return null;
        }

        OnMedkitLoadingTimeChanged.Invoke(1.0f, false);

        _isHealingLocked = false;

        yield return null;
    }

    public void OnHealed(float amountHealed)
    {
        _currentHealth += amountHealed;

        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        OnHealthChanged.Invoke(_currentHealth / _maxHealth);

        _isHealingLocked = false;

        OnMedkitLoadingTimeChanged.Invoke(0.0f, true);

        OnCharacterGotHealed.Invoke();

        StartCoroutine(UpdateMedkitLoading());
    }

    public void OnHit(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            OnDeath();
            OnHealthChanged.Invoke(0f);
            return;
        }

        OnCharacterGotHurt.Invoke();

        OnHealthChanged.Invoke(_currentHealth / _maxHealth);
    }

    public void OnDeath()
    {
        OnCharacterDied.Invoke();
        Debug.Log("Character Died");
    }

    public void LockAttack(bool isLocked)
    {
        _isAttackLocked = isLocked;
    }

    public void LockHeal(bool isLocked)
    {
        _isHealingLocked = isLocked;
    }
}
