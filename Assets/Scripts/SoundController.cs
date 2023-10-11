using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips;

    private AudioSource musicSource;
    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        PlayRandomMusic();
    }
    private void PlayRandomMusic()
    {
        var clip = clips[Random.Range(0, clips.Count)];
        musicSource.clip = clip;
        musicSource.Play();
    }
}
