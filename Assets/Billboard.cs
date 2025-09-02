using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private bool isDead = false;
    private CharacterData character;
    private Coroutine deathCoroutine;

    void Start()
    {
        character = GameManager.Instance.StateManager.Character;
        character.OnHealthChanged += SetDead;
    }

    void OnDestroy()
    {
        if (character != null)
        {
            character.OnHealthChanged -= SetDead;
        }
    }


    void LateUpdate()
    {
        if (!isDead)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }

    public void SetDead()
    {
        if (character.HealthState == HealthState.Dead && !isDead)
        {
            isDead = true;
            transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 0f);
        }
        else if (character.HealthState != HealthState.Dead && isDead)
        {
            isDead = false;
        }
    }
}
