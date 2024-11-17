using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;

    [Header("Object Pool Tags")]
    public string audioPoolTag = "AudioSource";
    public string musicPoolTag = "AudioSourceMusic";
    public string ambiencePoolTag = "AudioSourceAmbience";

    [Header("Audio Categories")]
    public Transform musicParent;
    public Transform ambienceParent;
    public Transform sfxParent;

    public GameObject audioSourcePrefab;
    public GameObject audioSourceMusicPrefab;
    public GameObject audioSourceAmbiencePrefab;
    private AudioSource currentMusicSource;
    private AudioSource currentAmbienceSource;
    private List<AudioSource> activeSFXSources = new List<AudioSource>();

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of FirebaseManager exists
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
        PoolingManager.Instance.CreatePool(audioPoolTag, audioSourcePrefab, 20, sfxParent);
        PoolingManager.Instance.CreatePool(musicPoolTag, audioSourcePrefab, 5, musicParent);
        PoolingManager.Instance.CreatePool(ambiencePoolTag, audioSourcePrefab, 5, ambienceParent);
    }

    // ===== MUSIC METHODS =====
    public void PlayMusic(AudioClipSettings settings)
    {
        // Check if the current music source is already playing the same clip
        if (currentMusicSource != null && currentMusicSource.isPlaying && currentMusicSource.clip == settings.clip)
        {
            // If the same music is already playing, do nothing
            return;
        }
        StopMusic(); // Stop previous music immediately before playing new one
        StartCoroutine(PlayAudio(settings, Vector3.zero, is3D: settings.is3D, isMusic: true, isAmbience: false));
    }

    public void StopMusic()
    {
        if (currentMusicSource != null && currentMusicSource.isPlaying)
        {
            currentMusicSource.Stop();
        }
    }

    public void StopMusicWithFade(float fadeDuration)
    {
        if (currentMusicSource != null && currentMusicSource.isPlaying)
        {
            StartCoroutine(FadeOutAndStop(currentMusicSource, fadeDuration));
        }
    }

    // ===== AMBIENCE METHODS =====
    public void PlayAmbience(AudioClipSettings settings)
    {
        StopAmbience(); // Stop previous ambience immediately before playing new one
        StartCoroutine(PlayAudio(settings, Vector3.zero, is3D: settings.is3D, isMusic: false, isAmbience: true));
    }

    public void StopAmbience()
    {
        if (currentAmbienceSource != null && currentAmbienceSource.isPlaying)
        {
            currentAmbienceSource.Stop();
        }
    }

    public void StopAmbienceWithFade(float fadeDuration)
    {
        if (currentAmbienceSource != null && currentAmbienceSource.isPlaying)
        {
            StartCoroutine(FadeOutAndStop(currentAmbienceSource, fadeDuration));
        }
    }

    // ===== SFX METHODS =====
    public void Play2DSFX(AudioClipSettings settings)
    {
        StartCoroutine(PlayAudio(settings, Vector3.zero, is3D: false, isMusic: false, isAmbience: false));
    }

    public void Play3DSFXAtLocation(AudioClipSettings settings, Vector3 location)
    {
        StartCoroutine(PlayAudio(settings, location, is3D: settings.is3D, isMusic: false, isAmbience: false));
    }

    public void StopSpecificSFX(AudioClip clip)
    {
        foreach (AudioSource source in activeSFXSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
                activeSFXSources.Remove(source);
                break;
            }
        }
    }

    public void StopSpecificSFXWithFade(AudioClip clip, float fadeDuration)
    {
        foreach (AudioSource source in activeSFXSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                StartCoroutine(FadeOutAndStop(source, fadeDuration));
                activeSFXSources.Remove(source);
                break;
            }
        }
    }

    // ===== HELPER METHODS =====
    private IEnumerator PlayAudio(AudioClipSettings settings, Vector3 position, bool is3D, bool isMusic, bool isAmbience)
    {
        string tag = isMusic ? musicPoolTag : isAmbience ? ambiencePoolTag : audioPoolTag;
        GameObject audioObject = PoolingManager.Instance.SpawnFromPool(tag, position, Quaternion.identity);
        if (audioObject == null) yield break;

        AudioSource audioSource = audioObject.GetComponent<AudioSource>();

        // Reset settings to avoid inheritance from other audio sources
        audioSource.loop = false;   // Reset loop to false initially to prevent unintended looping
        audioSource.volume = 1f;    // Reset volume to default
        audioSource.pitch = 1f;     // Reset pitch to default
        audioSource.spatialBlend = 0f; // Reset to 2D unless set to 3D
        audioSource.minDistance = 1f; // Default min distance
        audioSource.maxDistance = 500f; // Default max distance
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Default rolloff mode

        // Configure settings based on the AudioClipSettings
        ConfigureAudioSource(audioSource, settings, is3D);

        audioSource.PlayDelayed(settings.delay);

        if (isMusic)
        {
            currentMusicSource = audioSource;
        }
        else if (isAmbience)
        {
            currentAmbienceSource = audioSource;
        }
        else
        {
            activeSFXSources.Add(audioSource);
        }

        // Handle fade in
        if (settings.fadeInDuration > 0)
        {
            yield return StartCoroutine(FadeIn(audioSource, settings.fadeInDuration));
        }

        yield return new WaitWhile(() => audioSource.isPlaying);

        // Handle fade out
        if (settings.fadeOutDuration > 0)
        {
            yield return StartCoroutine(FadeOut(audioSource, settings.fadeOutDuration));
        }

        if (!isMusic && !isAmbience)
        {
            activeSFXSources.Remove(audioSource);
        }
        if(!isMusic)
            PoolingManager.Instance.ReturnToPool(audioPoolTag, audioObject);
    }

    private void ConfigureAudioSource(AudioSource audioSource, AudioClipSettings settings, bool is3D)
    {
        audioSource.clip = settings.clip;
        audioSource.volume = settings.volume;
        audioSource.pitch = settings.pitch;
        audioSource.outputAudioMixerGroup = settings.mixerGroup;
        audioSource.loop = settings.loop;
        audioSource.spatialBlend = is3D ? 1f : 0f;  // 3D or 2D

        if (is3D)
        {
            audioSource.minDistance = settings.minDistance;
            audioSource.maxDistance = settings.maxDistance;
            audioSource.rolloffMode = settings.rolloffMode;
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        float targetVolume = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume for next use
    }

    private IEnumerator FadeOutAndStop(AudioSource audioSource, float duration)
    {
        yield return StartCoroutine(FadeOut(audioSource, duration));
        audioSource.Stop();
    }

    [Button]
    public void SFXTest(AudioClipSettings clip) { 
        Play2DSFX(clip);
    }
    [Button]
    public void MusicTest(AudioClipSettings clip)
    {
        PlayMusic(clip);
    }

}
