using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipSettings", menuName = "Audio System/Audio Clip Settings")]
public class AudioClipSettings : ScriptableObject
{
    [Title("Audio Configuration")]
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public AudioMixerGroup mixerGroup;
    public bool loop = false;
    public float fadeInDuration = 0f;
    public float fadeOutDuration = 0f;
    public float delay = 0f;

    // 3D Audio Settings
    public bool is3D = false;                 // Whether the sound is 3D or 2D
    [ShowIf("is3D"),FoldoutGroup("3D Settings")] public float minDistance = 1f;            // The minimum distance at which the sound starts fading
    [ShowIf("is3D"), FoldoutGroup("3D Settings")] public float maxDistance = 500f;          // The maximum distance at which the sound is audible
    [ShowIf("is3D"), FoldoutGroup("3D Settings")] public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear; // How the sound fades over distance

    public void PlaySound2D() {
        AudioHelper.Play2DSFX(this);
    }
    public void PlayMusic()
    {
        AudioHelper.PlayMusic(this);
    }
}
