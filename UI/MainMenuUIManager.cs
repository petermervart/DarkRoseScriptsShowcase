using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Options")]

    [SerializeField]
    private TMP_Dropdown _graphicsDropdown;

    [SerializeField]
    private GameObject _optionsUIGroup;

    [SerializeField]
    private GameObject _creditsUIGroup;

    [SerializeField]
    private GameObject _logo;

    [SerializeField]
    private Slider _musicVolumeSlider;

    [SerializeField]
    private Slider _gameplayVolumeSlider;

    [SerializeField]
    private OptionsConfig _options;

    [Header("Main Menu")]
    [SerializeField]
    private GameObject _mainMenuUIGroup;

    private void Start()
    {
        StartCoroutine(AfterStartCoroutine());
    }

    IEnumerator AfterStartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        InitSettings();
    }

    public void InitSettings()
    {
        _gameplayVolumeSlider.value = _options.CurrentGameplayVolume;
        _musicVolumeSlider.value = _options.CurrentMusicVolume;
        _options.OnChangedGameplayVolume?.Invoke(_options.CurrentGameplayVolume);
        _options.OnChangedMusicVolume?.Invoke(_options.CurrentMusicVolume);
        _graphicsDropdown.value = _options.CurrentGraphicsSettings;
    }

    public void OnGameplayVolumeChanged()
    {
        _options.CurrentGameplayVolume = _gameplayVolumeSlider.value;
        _options.OnChangedGameplayVolume?.Invoke(_options.CurrentGameplayVolume);
    }

    public void OnMusicVolumeChanged()
    {
        _options.CurrentMusicVolume = _musicVolumeSlider.value;
        _options.OnChangedMusicVolume?.Invoke(_options.CurrentMusicVolume);
    }

    public void OnChangeToMainMenu()
    {
        _optionsUIGroup.SetActive(false);
        _creditsUIGroup.SetActive(false);
        _mainMenuUIGroup.SetActive(true);
        _logo.SetActive(true);
    }

    public void OnChangeToOptions()
    {
        _optionsUIGroup.SetActive(true);
        _mainMenuUIGroup.SetActive(false);
    }

    public void OnChangeToCredits()
    {
        _creditsUIGroup.SetActive(true);
        _mainMenuUIGroup.SetActive(false);
        _logo.SetActive(false);
    }
}
