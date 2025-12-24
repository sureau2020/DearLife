using UnityEngine;
using TMPro;

[RequireComponent(typeof(InfoItem))]
public class ApparenceNameUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI apparenceName;

    private InfoItem infoItem;

    void Awake()
    {
        infoItem = GetComponent<InfoItem>();
    }

    void OnEnable()
    {
        infoItem.EditCompleted += OnEditCompleted;
    }

    void OnDisable()
    {
        infoItem.EditCompleted -= OnEditCompleted;
    }

    private void OnEditCompleted(string finalValue)
    {
        if (apparenceName != null)
            apparenceName.text = finalValue;
    }
}