using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI contentText;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;


    public void Show(ConfirmData data)
    {
        SoundManager.Instance.PlaySfx("Delete");
        gameObject.SetActive(true);

        contentText.text = data.content;

        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();

        confirmBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySfx("Click");
            data.onConfirm?.Invoke();
            Close();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySfx("Click");
            data.onCancel?.Invoke();
            Close();
        });
    }

    void Close()
    {
        gameObject.SetActive(false);
    }
}
