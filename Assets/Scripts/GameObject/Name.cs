using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Name : MonoBehaviour
{
    [SerializeField]private TextMeshPro panel;
    
    void Start()
    {
        panel.text = GameManager.Instance.StateManager.Character.Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
