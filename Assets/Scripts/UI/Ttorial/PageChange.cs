using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageChange : MonoBehaviour
{
    public string[] Lines;
    private int currentLine = -1;
    [SerializeField] private TextMeshProUGUI line;
    [SerializeField] private Button NextPage;
    [SerializeField] private GameObject panel;

    void Start()
    {
        Next();
    }

    public void Next()
    {
        if (currentLine < Lines.Length - 1)
        {
            currentLine++;
            line.text = Lines[currentLine];
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
