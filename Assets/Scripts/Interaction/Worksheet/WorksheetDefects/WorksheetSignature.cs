using CreamyCrescentRolls.RoomGameWorksheetVars;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorksheetSignature : MonoBehaviour, IWorksheetConstruct
{
    [SerializeField] private List<Sprite> signatureSprites = new();
    [SerializeField] private List<string> signatureNames = new();


    private Image image;
    private TMP_Text text;

    public void MakeDefect(int count)
    {
        if (count == 0)
            return;

        if (image == null)
            image = GetComponentInChildren<Image>();

        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        if (text == null || image == null)
        {
            Debug.LogError("[WorksheetSignature] Signature dosent contains all required components");
            return;
        }

        if (image.sprite == null)
        {
            Debug.LogError("[WorksheetSignature] Image of signature dosent contains sprite");
            return;
        }

        signatureNames.Remove(text.text);
        signatureSprites.Remove(image.sprite);

        if (count % 2 == 0)
        {
            text.text = signatureNames[UnityEngine.Random.Range(0, signatureSprites.Count)];
        }  
        else
        {
            image.sprite = signatureSprites[UnityEngine.Random.Range(0, signatureSprites.Count)];
        }
    }
}
