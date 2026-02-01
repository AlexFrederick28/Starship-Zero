using NUnit.Framework;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource soundObject;
    [SerializeField] private AudioSource dialogueClip;

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
}
