using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettingsManager : MonoBehaviour
{
    public float mainVolume = 1.0f;
    public float soundEffectVolume = 0.5f;
    public float musicVolume = 0.5f;

    public Slider mainVolumeSlider;
    public Slider soundEffectVolumeSlider;
    public Slider musicVolumeSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
