using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        // Keep this obj even when we go to next scene
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Destroy duplicate
        else if(instance && instance != this)
        {
            Destroy(gameObject);
        }

    }

    public void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }

    void Update()
    {
        
    }
}
