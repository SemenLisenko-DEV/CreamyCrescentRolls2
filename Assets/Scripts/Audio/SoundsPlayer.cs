using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SoundsPlayer : MonoBehaviour
{
    private AudioClip _defaultClip;

    public AudioClip DefaultClip 
    {
        get
        {
            if (_defaultClip == null)
                _defaultClip = Resources.Load<AudioClip>("Audio/DefaultSound");

            return _defaultClip;
        }
    }

    private static SoundsPlayer Instance;

    private AudioSource _audioSource;

    [SerializeField] private List<AudioClip> _clips = new();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static void PlayByInstance(string name)
    {
        if(Instance == null)
        {
            Debug.LogWarning("[SoundsPlayer] Instance arent found");
            return;
        }

        Instance.PlaySound(name);
    }

    public void PlaySound(string name) => _audioSource.PlayOneShot(GetClip(name));

    public AudioClip GetClip(string name)
    {
        foreach (AudioClip clip in _clips)
        {
            if(clip.name == name)
                return clip;
        }

        return DefaultClip;
    }
}
