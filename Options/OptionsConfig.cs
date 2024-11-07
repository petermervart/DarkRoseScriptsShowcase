using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Options Config", fileName = "OptionsConfig")]

public class OptionsConfig : ScriptableObject
{
    [Header("Sound")]
    public float CurrentGameplayVolume;
    public float CurrentMusicVolume;

    [Header("Graphics")]
    public int CurrentGraphicsSettings = 0;
    public RenderPipelineAsset LowSettings;
    public RenderPipelineAsset MediumSettings;
    public RenderPipelineAsset HighSettings;

    public delegate void ChangedGameplayVolume(float newVolume);
    public ChangedGameplayVolume OnChangedGameplayVolume;

    public delegate void ChangedMusicVolume(float newVolume);
    public ChangedMusicVolume OnChangedMusicVolume;

    public void OnChangedSettings(int newSettingsIndex)
    {
        QualitySettings.SetQualityLevel(newSettingsIndex, true);
        CurrentGraphicsSettings = newSettingsIndex;
    }
}
