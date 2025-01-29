using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    private AudioSource _source;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    public bool loop = false;
    [Range(0f, 1.5f)]
    public float pitch = 1f;
    [Range(0f, 0.5f)]
    public float volumeRandomness = 0.1f;
    [Range(0f, 0.5f)]
    public float pitchRandomness = 0.1f;
    public void SetSource(AudioSource source)
    {
        _source = source;
        _source.clip = clip;
    }

    public void Play()
    {
        _source.volume = volume * (1 + Random.Range(-volumeRandomness / 2f, volumeRandomness / 2f));
        _source.loop = loop;
        _source.pitch = pitch * (1 + Random.Range(-pitchRandomness / 2f, pitchRandomness / 2f));
        _source.Play();
    }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SoundManager in the scene");
        }
        else
        {
            instance = this;
        }
    }
    
    [SerializeField] private Sound[] sounds;

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }
    
    public void PlaySound(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning("SoundManager: Sound not found in list, " + soundName);
    }
}
