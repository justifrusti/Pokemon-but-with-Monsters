using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTrigger : MonoBehaviour
{
    public AudioManager manager;

    public string audioToPlay;

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
    }

    public void Start()
    {
        PlayAudio();
    }

    public void PlayAudio()
    {
        manager.PlayClip(audioToPlay);
    }
}
