using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource soundObject;

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
}
