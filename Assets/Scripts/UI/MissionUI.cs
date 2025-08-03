using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    // Start is called before the first frame update

    private TMPro.TextMeshProUGUI title;
    private TMPro.TextMeshProUGUI ddl;
    private Button completeButton;
    private Button cancelButton;
    private MissionData missionData;

    void Start()
    {
        title = gameObject.transform.Find("title").GetComponent<TMPro.TextMeshProUGUI>();
        ddl = gameObject.transform.Find("ddl").GetComponent<TMPro.TextMeshProUGUI>();
        completeButton = gameObject.transform.Find("CompleteButton").GetComponent<Button>();
        cancelButton = gameObject.transform.Find("CancelButton").GetComponent<Button>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
