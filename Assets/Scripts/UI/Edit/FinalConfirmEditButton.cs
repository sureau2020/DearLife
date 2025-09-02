
using UnityEngine;

public class FinalConfirmEditButton : MonoBehaviour
{
    public void OnClick()
    {
        SoundManager.Instance.PlaySfx("LittleType");
        BootSceneManager.Instance.ConfirmCharacter();
    }
}
