using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestText : MonoBehaviour
{
    [SerializeField] private string[] testScript;
    [SerializeField] private TMP_Text text;
    private int index = 0;

    public void ShowNextScript()
    {
        text.text = testScript[index];
        index++;
    }
}