using NUnit.Framework;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource musicObject;
    public AudioClip[] mainMusic;
    public AudioClip[] battleMusic;
    public AudioClip[] tutorialMusic;
    [SerializeField] private AudioSource soundObject;
    [SerializeField] private AudioSource dialogueClip;

    public float defaultMinPitch = 1.0f;
    public float defaultMaxPitch = 1.8f;

    // sound effects audio
    public AudioClip[] soundEffectsArray; // 0 = equip weapon, 1 = remove weapon, 2 = UI interact, 3 = xp, 4 = crafting bench, 5 = card  

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (instance != this)
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
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

    public void PlayDialogueSoundClip(AudioClip clip, Transform transform, float volume, bool isSound2D, bool randomisePitch, float minPitch, float maxPitch)
    {
        // destroy any existing clip if the player is spamming next dialogue
        Destroy(dialogueClip);
        dialogueClip = Instantiate(soundObject, transform.position, Quaternion.identity);
        dialogueClip.clip = clip;
        dialogueClip.volume = volume;
        if (isSound2D == true) { dialogueClip.spatialBlend = 0f; }
        else { dialogueClip.spatialBlend = 1f; }
        if (randomisePitch == true) { dialogueClip.pitch = Random.Range(minPitch, maxPitch); }
        dialogueClip.Play();
        float clipLength = dialogueClip.clip.length;
        Destroy(dialogueClip.gameObject, clipLength);
    }

    public IEnumerator PlayMusicClipCoroutine(AudioClip[] clip, Transform transform, float volume, bool repeat)
    {
        // Gamestate.cs handles music calls
        if (musicObject == null)
        {
            // only need one music object in the scene
            musicObject = Instantiate(soundObject, transform.position, Quaternion.identity);
        }
        
        // 2D sound
        musicObject.spatialBlend = 0f;

        if (repeat == true)
        {
            bool newClip = false;
            musicObject.volume = volume;

            double nextStartTime = AudioSettings.dspTime;

            while (repeat == true)
            {
                if (newClip == false)
                {
                    AudioClip nextClip = clip[Random.Range(0, clip.Length)];

                    musicObject.clip = nextClip;
                    musicObject.PlayScheduled(nextStartTime);

                    nextStartTime += nextClip.length;

                    newClip = true;
                }

                float clipLength = musicObject.clip.length;
                yield return new WaitForSeconds(clipLength);
                newClip = false;
            }
        }
        else
        {
            musicObject.clip = clip[Random.Range(0, clip.Length)];
            musicObject.volume = volume;
            musicObject.Play();
            float clipLength = musicObject.clip.length;
            yield return new WaitForSeconds(clipLength);
        }
    }

    public float SoundVolume()
    {
        float soundEffectVolume = 0.5f; 

        if (UIManager.instance.MasterSlider != null || UIManager.instance.SoundsEffectsSlider != null)
        {
            soundEffectVolume = UIManager.instance.MasterSlider.value * UIManager.instance.SoundsEffectsSlider.value;
        }
        else
        {
            Debug.Log("cannot find master slider or sound effect slider!");
        }

        return soundEffectVolume;
    }

    public float MusicVolume()
    {
        float soundEffectVolume = 0.5f;

        if (UIManager.instance.MasterSlider != null || UIManager.instance.MusicSlider != null)
        {
            soundEffectVolume = UIManager.instance.MasterSlider.value * UIManager.instance.MusicSlider.value;
        }
        else
        {
            Debug.Log("cannot find master slider or music slider!");
        }

        return soundEffectVolume;
    }

    public void PlayUISound(float pitch) // buttons - on click, play this function
    {
        // if need be add another array for UI and random range 0 - arrary.count for variety
        PlaySoundClip(soundEffectsArray[2], transform, SoundVolume(), true, true, pitch, pitch); // UI sound
    }

}
