using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // 單例模式確保只有一個 MusicManager
    public AudioSource audioSource; // 音樂的 AudioSource

    public AudioClip musicClip; // 要播放的音樂剪輯

    private void Awake()
    {
        // 確保只會有一個 MusicManager 實例，不重複播放音樂
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切換場景時不銷毀音樂
        }
        else
        {
            Destroy(gameObject); // 如果已經有一個 MusicManager，銷毀這個實例
        }
    }

    private void Start()
    {
        if (!audioSource.isPlaying) // 如果音樂沒播放，播放它
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