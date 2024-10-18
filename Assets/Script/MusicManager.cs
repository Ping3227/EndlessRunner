using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; 
    public AudioSource audioSource;

    public AudioClip musicClip;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!audioSource.isPlaying) //
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        if (musicClip != null && audioSource.clip != musicClip)
        {
            audioSource.clip = musicClip;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}