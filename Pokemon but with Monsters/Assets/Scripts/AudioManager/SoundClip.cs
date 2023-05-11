using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Custom Datasets/Audio Clip", fileName = "New Audio Clip")]
public class SoundClip : ScriptableObject
{
    [HideInInspector] public AudioSource source;

    public string clipName;
    [Space]
    public AudioClip clip;
    public AudioMixerGroup group;
    [Space]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    [Space]
    public bool loop;
    public bool playOnAwake;
}