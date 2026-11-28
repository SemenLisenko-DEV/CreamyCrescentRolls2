using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPauser : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private float _minVolume = -1;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_minVolume == -1)
        {
            if (Time.timeScale == 0)
                _audioSource.volume = Mathf.Clamp(_audioSource.volume - Time.unscaledDeltaTime, 0, 1);
            else
                _audioSource.volume = Mathf.Clamp(_audioSource.volume + Time.unscaledDeltaTime, 0, 1);

            if (_audioSource.volume <= 0.05f)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
        }
        else
        {
            if (Time.timeScale == 0)
                _audioSource.volume = Mathf.Clamp(_audioSource.volume - Time.unscaledDeltaTime, _minVolume, 1);
            else
                _audioSource.volume = Mathf.Clamp(_audioSource.volume + Time.unscaledDeltaTime, _minVolume, 1);
        }
    }
}
