using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("PauseMenu / Options")]

    [SerializeField]
    private OptionsConfig _options;

    [SerializeField]
    private TMP_Dropdown _graphicsDropdown;

    [SerializeField]
    private GameObject _pauseMenuUIGroup;

    [SerializeField]
    private GameObject _menuButtonsUIGroup;

    [SerializeField]
    private GameObject _optionsUIGroup;

    [SerializeField]
    private Slider _musicVolumeSlider;

    [SerializeField]
    private Slider _gameplayVolumeSlider;

    public delegate void ChangedPauseGameState(bool isPaused);

    public ChangedPauseGameState OnChangedPauseGameState;

    [Header("InfoText")]

    [SerializeField]
    private TextMeshProUGUI _infoText;

    [Header("Crosshair")]

    [SerializeField]
    private GameObject _crosshairUIGroup;

    [SerializeField]
    private TextMeshProUGUI _crosshairUIHelperText;

    [Header("Health")]

    [SerializeField]
    private Image _healthForeground;

    [SerializeField]
    private Image _medkitIconForeground;

    [SerializeField]
    private Color _medkitNormalColor;

    [SerializeField]
    private Color _medkitLoadingColor;

    [Header("Looting")]

    [SerializeField] 
    private GameObject _lootingUIGroup;

    [SerializeField] 
    private Image _lootingBar;

    [SerializeField] 
    private LootingConfig _lootingConfig;

    [SerializeField] 
    private GameObject _lootedItemShowPrefab;

    [SerializeField] 
    private Transform _parentOfLootedShows;

    [Header("Crafting")]

    [SerializeField]
    private GameObject _craftingUIGroup;

    [Header("Building")]

    [SerializeField]
    private GameObject _buildingUIGroup;

    [SerializeField]
    private Color _pickedTrapColor;

    [SerializeField]
    private Color _normalTrapColor;

    [SerializeField]
    private List<Image> _trapsIconsGroup;

    private bool _isShowingHelperText = false;
    private float _timeShowingHelperText = 0f;
    private int _currentTrapIndex = 0;
    private bool _isMedkitColorAlreadyChanged = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        StartCoroutine(AfterStartCoroutine());
    }

    IEnumerator AfterStartCoroutine()
    {
        yield return new WaitForEndOfFrame();
        InitSettings();
    }

    public void OnExitScene()
    {
        Time.timeScale = 1;
        SetCursorLock(false);
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

    public void OnPauseMenuOpenOptions()
    {
        _optionsUIGroup.SetActive(true);
        _menuButtonsUIGroup.SetActive(false);
    }

    public void OnPauseMenuCloseOptions()
    {
        _optionsUIGroup.SetActive(false);
        _menuButtonsUIGroup.SetActive(true);
    }

    public void ShowCrosshairText(string text)
    {
        _crosshairUIHelperText.SetText(text);
    }

    public void ShowCrosshairUI(bool show)
    {
        _crosshairUIGroup.gameObject.SetActive(show);
    }

    public void ShowLootingUI(bool show)
    {
        _lootingUIGroup.SetActive(show);
    }

    public void StopTime(bool shouldStop)
    {
        if (shouldStop)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void OnPauseMenuChanged(bool isOpened)
    {
        _pauseMenuUIGroup.SetActive(isOpened);

        OnChangedPauseGameState?.Invoke(isOpened);

        if (!isOpened)
            OnPauseMenuCloseOptions();

        SetCursorLock(!isOpened);
        StopTime(isOpened);
    }

    public void OnUpdateLootingBar(float fillAmount)
    {
        _lootingBar.fillAmount = fillAmount;
    }

    public void OnPressedBuilding(bool isPressed)
    {
        _buildingUIGroup.SetActive(isPressed);
    }

    public void OnChangedCurrentBuildingTrap(int index)
    {
        _trapsIconsGroup[_currentTrapIndex].color = _normalTrapColor;
        _trapsIconsGroup[index].color = _pickedTrapColor;
        _currentTrapIndex = index;
    }

    public void OnChangedLootingState(bool isLooting)
    {
        ShowLootingUI(isLooting);
        ShowCrosshairUI(!isLooting);
        if (!isLooting)
        {
            ShowCrosshairText("");
        }
    }

    public void OnNewItemLooted(string text, Sprite icon)
    {
        GameObject instantiatedLootedShow = Instantiate(_lootedItemShowPrefab, Vector3.zero, Quaternion.identity);

        if (instantiatedLootedShow.TryGetComponent<LootedItemShow>(out var lootItemShow))
        {
            lootItemShow.SetTextAndIcon(text, icon);
        }

        instantiatedLootedShow.transform.SetParent(_parentOfLootedShows, false);
    }

    public void OnShowHelperText(string text, float timeToShow)
    {
        ShowCrosshairText(text);
        _isShowingHelperText = true;
        _timeShowingHelperText = 0f;
        StartCoroutine(UpdateHelperText(timeToShow));
    }

    public void OnShowInfoText(string text, float timeToShow)
    {
        _infoText.SetText(text);
        StartCoroutine(UpdateInfoText(timeToShow));
    }

    private IEnumerator UpdateInfoText(float timeToShow)
    {
        yield return new WaitForSeconds(timeToShow);

        _infoText.SetText("");
    }

    private IEnumerator UpdateHelperText(float timeToShow)
    {
        while (_isShowingHelperText && _timeShowingHelperText < timeToShow)
        {
            _timeShowingHelperText += Time.deltaTime;
            yield return null;
        }

        _isShowingHelperText = false;
        _timeShowingHelperText = 0f;
        ShowCrosshairText("");
    }

    public void OnUpdateHealthBar(float newFillAmount)
    {
        _healthForeground.fillAmount = newFillAmount;
    }

    public void OnMedkitLoadingTime(float medkitFillAmount, bool shouldChangeColor)
    {
        Debug.Log(medkitFillAmount);

        _medkitIconForeground.fillAmount = medkitFillAmount;

        if(!_isMedkitColorAlreadyChanged && shouldChangeColor)
        {
            _medkitIconForeground.color = _medkitLoadingColor;
            _isMedkitColorAlreadyChanged = true;
        }
        else if (_isMedkitColorAlreadyChanged && !shouldChangeColor)
        {
            _medkitIconForeground.color = _medkitNormalColor;
            _isMedkitColorAlreadyChanged = false;
        }
    }

    public void OnPressedInventory(bool isPressed)
    {
        _craftingUIGroup.SetActive(isPressed);
        _crosshairUIGroup.SetActive(!isPressed);
        SetCursorLock(!isPressed);
    }

    public void SetCursorLock(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
