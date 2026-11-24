using CreamyCrescentRolls.RoomGameWorksheetVars;
using TMPro;
using UnityEngine;

public class WorksheetText : MonoBehaviour, IWorksheetConstruct 
{
    private static readonly char[] _randomDefectsChars = {'|', '#', '@', '+', ')', '('};

    private TMP_Text _textField;

    public void MakeDefect(int count)
    {
        if (_textField == null)
            _textField = GetComponentInChildren<TMP_Text>();

        if (_textField.text.Length == 0 || count == 0)
            return;

        char[] chars = _textField.text.ToCharArray();
        int[] shakedIndexes = new int[chars.Length];

        for (int i = 0; i < shakedIndexes.Length; i++)
        {
            shakedIndexes[i] = i;
        }

        for (int i = 0; i < shakedIndexes.Length; i++)
        {
            int j = UnityEngine.Random.Range(i, shakedIndexes.Length);
            (shakedIndexes[i], shakedIndexes[j]) = (shakedIndexes[j], shakedIndexes[i]);
        }

        for (int i = 0; i < count && i < chars.Length; i++)
        {
            int j = UnityEngine.Random.Range(0, _randomDefectsChars.Length);
            chars[shakedIndexes[i]] = _randomDefectsChars[j];
        }

        _textField.text = new(chars);
    }

}
