using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [Header("Info Text Settings")]

    [SerializeField]
    private string _almostNightText;
    [SerializeField]
    private float _almostNightTimeBeforeNightInSeconds;
    [SerializeField]
    private float _almostNightTextShowTime;

    [SerializeField]
    private string _veryCloseToNightText;
    [SerializeField]
    private float _veryCloseToNightTimeBeforeNightInSeconds;
    [SerializeField]
    private float _veryCloseToNightTextShowTime;

    [SerializeField] 
    private float _dayDurationTimeMinutes;

    [SerializeField] 
    private float _nightDurationTimeMinutes;

    [SerializeField] 
    private Transform _sunObject;

    [SerializeField] 
    private float _ySunRotation;

    [SerializeField] 
    private float _zSunRotation;

    [SerializeField] 
    private float _dayStartSunRotation = 15f;

    [SerializeField] 
    private float _nightStartSunRotation = 180f;

    [SerializeField]
    UnityEvent OnSurvived = new UnityEvent();

    [SerializeField]
    UnityEvent OnNightStarted = new UnityEvent();

    [SerializeField]
    UnityEvent<string, float> OnChangeInfoText = new UnityEvent<string, float>();

    private float _nightEndSunRotation;

    private float _currentTime;

    private float _currentSunRotation;
    private float _dayRotationTick;
    private float _nightRotationTick;

    private bool _isNight;

    private bool _showedAlmostNightText = false;
    private bool _showedVeryCloseToNightText = false;

    public void SetTimeToDay()
    {
        _currentTime = 0f;
        _currentSunRotation = _dayStartSunRotation;
        _isNight = false;
    }

    public void SetTimeToNight()
    {
        _currentTime = 0f;
        _currentSunRotation = _nightStartSunRotation;
        _isNight = true;
        OnNightStart();
    }

    public void OnNightStart()
    {
        AgentSpawner.Instance.StartSpawnEnemies();
        OnNightStarted.Invoke();
        Debug.Log("Night Started");
    }

    public void UpdateTime()
    {
        _currentTime += Time.deltaTime;

        if (!_isNight)
        {
            _currentSunRotation = _dayStartSunRotation + (_currentTime * _dayRotationTick);

            if (_currentSunRotation > _nightStartSunRotation)
            {
                SetTimeToNight();
            }

            if(!_showedAlmostNightText && (_currentTime > ((_dayDurationTimeMinutes * 60.0f) - _almostNightTimeBeforeNightInSeconds)))
            {
                _showedAlmostNightText = true;
                OnChangeInfoText.Invoke(_almostNightText, _almostNightTextShowTime);
            }

            if (!_showedVeryCloseToNightText && (_currentTime > ((_dayDurationTimeMinutes * 60.0f) - _veryCloseToNightTimeBeforeNightInSeconds)))
            {
                _showedVeryCloseToNightText = true;
                OnChangeInfoText.Invoke(_veryCloseToNightText, _veryCloseToNightTextShowTime);
            }
        }
        else
        {
            _currentSunRotation = _nightStartSunRotation + (_currentTime * _nightRotationTick);

            if (_currentSunRotation > _nightEndSunRotation)
            {
                SetTimeToDay();
                OnSurvived.Invoke();
            }
        }

        _sunObject.localEulerAngles = new Vector3(_currentSunRotation, _ySunRotation, _zSunRotation);
    }

    void Start()
    {
        _nightEndSunRotation = _dayStartSunRotation + 360f;
        _dayRotationTick = (_nightStartSunRotation - _dayStartSunRotation) / (_dayDurationTimeMinutes * 60);
        _nightRotationTick = (_nightEndSunRotation - _nightStartSunRotation) / (_nightDurationTimeMinutes * 60);

        SetTimeToDay();
    }

    void Update()
    {
        UpdateTime();
    }
}
