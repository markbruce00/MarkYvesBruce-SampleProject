using UnityEngine;

public static class AudioHelper
{
    // Play Music
    public static void PlayMusic(AudioClipSettings settings)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.PlayMusic(settings);
        }
    }

    // Stop Music
    public static void StopMusic()
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopMusic();
        }
    }

    // Stop Music with Fade
    public static void StopMusicWithFade(float fadeDuration)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopMusicWithFade(fadeDuration);
        }
    }

    // Play Ambience
    public static void PlayAmbience(AudioClipSettings settings)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.PlayAmbience(settings);
        }
    }

    // Stop Ambience
    public static void StopAmbience()
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopAmbience();
        }
    }

    // Stop Ambience with Fade
    public static void StopAmbienceWithFade(float fadeDuration)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopAmbienceWithFade(fadeDuration);
        }
    }

    // Play 2D SFX
    public static void Play2DSFX(AudioClipSettings settings)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.Play2DSFX(settings);
        }
    }

    // Play 3D SFX at location
    public static void Play3DSFXAtLocation(AudioClipSettings settings, Vector3 location)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.Play3DSFXAtLocation(settings, location);
        }
    }

    // Stop Specific SFX
    public static void StopSpecificSFX(AudioClip clip)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopSpecificSFX(clip);
        }
    }

    // Stop Specific SFX with Fade
    public static void StopSpecificSFXWithFade(AudioClip clip, float fadeDuration)
    {
        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopSpecificSFXWithFade(clip, fadeDuration);
        }
    }
}
