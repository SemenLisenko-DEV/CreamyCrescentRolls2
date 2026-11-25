using UnityEngine;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private string _volumeGroupName;
    [SerializeField] private AudioMixer _audioMixer;

    public void SetVolume(float value)
    {
        _audioMixer.SetFloat(_volumeGroupName, GetVolumeInDB(value));
    }

    public float GetVolumeInDB(float procent) => Mathf.Log10(Mathf.Max(procent, 0.0001f)) * 20f;
}
