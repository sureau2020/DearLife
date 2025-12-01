using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour
{
    [SerializeField] GameObject character;
    [SerializeField] CameraFocus cameraFocus;
    private Vector3 characterPos = new Vector3(0.7f, 5f, -0.12f);
    private Vector3 cameraPos = new Vector3(0.7f, 7f, 0.2f);
    public void ChangeShowingState()
  {
        if (gameObject.activeSelf)
        {
            SoundManager.Instance.PlaySfx("PanelMove");
            cameraFocus.ResetCamera();
            gameObject.SetActive(false);
            
        }
        else
        {
            SoundManager.Instance.PlaySfx("PanelMove");
            character.transform.position = characterPos;
            cameraFocus.FocusOnTarget(cameraPos);
            gameObject.SetActive(true);
        }

    }
}
