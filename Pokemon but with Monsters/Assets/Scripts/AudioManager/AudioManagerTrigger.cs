using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTrigger : MonoBehaviour
{
    public AudioManager manager;

    public string audioToPlay;

    public void Start()
    {
        PlayAudio();
    }

    public void PlayAudio()
    {
        manager.PlayClip(audioToPlay);
    }
}
