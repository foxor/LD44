using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BillEntryController : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Cost;

    public void Setup(string EntryText, int count, int costPerCount)
    {
        Label.text = $"{EntryText} (x{count}): ";
        Cost.text = $"{costPerCount / 100f:C2}";
    }
}
