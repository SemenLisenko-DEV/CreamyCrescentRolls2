using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using CreamyCrescentRolls.RoomGame.WorksheetVars;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorksheetNumber : MonoBehaviour, IWorksheetConstruct
{
    [SerializeField] private string targetTag;
    private TMP_Text text;

    public void MakeDefect(int count, List<GameObject> othersConstructs)
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();


        GameObject targetNumberObject = othersConstructs.Find((x) => x.tag == targetTag);

        if (text == null || targetNumberObject == null)
        {
            Debug.LogError("[WorksheetNumber] WorksheetNumber arent found all required components");
            return;
        }

        TMP_Text targetText = targetNumberObject.GetComponentInChildren<TMP_Text>();

        if (targetText == null)
        {
            Debug.LogError("[WorksheetNumber] Target text arent found");
            return;
        }

        char[] chars = targetText.text.Split(' ')[^1].ToCharArray();

        if (count != 0)
        {
            List<int> digits = new();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= '0' && chars[i] <= '9')
                    digits.Add(i);
            }

            ListRandomer.Shuffle(digits);

            for (int i = 0; i < digits.Count && i < count; i++)
            {
                chars[digits[i]] = (char)('0' + ((chars[digits[i]] - '0' + 1) % 10));
            }
        }

        text.text += new string(chars);
    }
}
