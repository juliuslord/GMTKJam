using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> audioClips;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayRandomAudioClip();
    }

    private void PlayRandomAudioClip()
    {
        int randomIndex = Random.Range(0, audioClips.Count);
        audioSource.clip = audioClips[randomIndex];
        audioSource.Play();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomAudioClip();
        }
    }
}
