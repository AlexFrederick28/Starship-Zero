using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettingsManager : MonoBehaviour
{
    public float mainVolume = 1.0f;
    public float soundEffectVolume = 0.5f;
    public float musicVolume = 0.5f;

    public AudioSource musicAudioSource;

    public Slider mainVolumeSlider;
    public Slider soundEffectVolumeSlider;
    public Slider musicVolumeSlider;

    [SerializeField] private AudioSource soundObject;

    public AudioClip[] soundEffectsArray; // 0 = equip weapon, 1 = remove weapon, 2 = UI interact, 3 = xp, 4 = crafting bench, 5 = card  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        musicAudioSource.Play();
    }

    private void Update()
    {
        musicAudioSource.volume = musicVolumeSlider.value * mainVolumeSlider.value;
    }

    public float SoundVolume()
    {
        float soundEffectVolume = 0.5f;

        soundEffectVolume = soundEffectVolumeSlider.value * mainVolumeSlider.value; 
        Debug.Log(soundEffectVolume);

        return soundEffectVolume;
    }

    public void PlaySoundClip(AudioClip clip, Transform transform, float volume, bool isSound2D, bool randomisePitch, float minPitch, float maxPitch)
    {
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        if (isSound2D == true) { audioSource.spatialBlend = 0f; }
        else { audioSource.spatialBlend = 1f; }
        if (randomisePitch == true) { audioSource.pitch = Random.Range(minPitch, maxPitch); }
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayUISound(float pitch) // buttons - on click, play this function
    {
        // if need be add another array for UI and random range 0 - arrary.count for variety
        PlaySoundClip(soundEffectsArray[2], transform, SoundVolume(), true, true, pitch, pitch); // UI sound
    }
}
