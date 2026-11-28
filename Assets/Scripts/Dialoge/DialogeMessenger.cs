using TMPro;
using UnityEngine;

public class DialogeMessenger : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private float _charTime = 0.1f;
    [SerializeField] private float _stayTime = 0.1f;
    [SerializeField] AudioSource _soundSource;

    private float _timerChar = 0f;
    private float _timerStay = 0f;

    public void DrawMessenger(string text)
    {
        textField.text = text;
        textField.maxVisibleCharacters = 0;
    }

    public void Update()
    {
        _timerChar = Mathf.Clamp(_timerChar + Time.deltaTime, 0, _charTime);

        if (_charTime <= _timerChar)
        {
            textField.maxVisibleCharacters = Mathf.Clamp(textField.maxVisibleCharacters + 1, 0, textField.text.Length);
            _timerChar = 0;
        }

        if (textField.maxVisibleCharacters == textField.maxVisibleCharacters)
        {
            _timerStay += Mathf.Clamp(_timerStay + Time.deltaTime, 0, _stayTime);
        }

        if (_stayTime <= _timerStay)
        {
            textField.text = string.Empty;
            textField.maxVisibleCharacters = 0;
        }
    }
}
