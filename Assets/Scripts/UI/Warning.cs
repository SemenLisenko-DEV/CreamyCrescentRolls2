using TMPro;
using UnityEngine;

public class Warning : MonoBehaviour
{
    [SerializeField] private TMP_Text _warningField;
    [SerializeField] private CanvasGroup _warningCanvasGroup;

    private float _alpha = 0;

    public void DrawWarning(string warningText)
    {
        if (_warningField == null)
        {
            Debug.LogError("[Warning] Warning field arent defined");
        }

        if (_warningField == null)
        {
            Debug.LogError("[Warning] Warning canvas group arent defined");
        }

        _alpha = 3;
        _warningField.text = warningText;
    }

    public void Update()
    {
        if (_warningCanvasGroup == null)
        {
            return;
        }

        _alpha -= Time.deltaTime;
        _warningCanvasGroup.alpha = _alpha;
    }
}
