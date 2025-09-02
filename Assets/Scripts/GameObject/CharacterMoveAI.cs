using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class CharacterMoveAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterData character;
    private bool isAlive = true;
    private bool isWaiting = false;
    [SerializeField] private CharacterBreatheComponent breatheComponent;

    public float walkRadius = 5f;     // ��뾶
    public float minWait = 2f;        // �ȴ�ʱ�䷶Χ
    public float maxWait = 5f;

    private bool isFocused = false;  

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        ChooseNextDestination();
        character = GameManager.Instance.StateManager.Character;
        character.OnHealthChanged += OnCharacterStateChangedHandler;
    }

    void OnDestroy()
    {
        if (character != null)
        {
            character.OnHealthChanged -= OnCharacterStateChangedHandler;
        }
    }

    void Update()
    {
        if (!isAlive) return;
        if (!isFocused) 
        {
            // ����Ŀ�� �� �ȴ�
            if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.2f)
            {
                StartCoroutine(WaitAndMove());
            }
        }

        if (!isFocused)  
        {
            UpdateSpriteDirection(agent.velocity);
        }
    }

    private void OnCharacterStateChangedHandler()
    {
        if (character.HealthState == HealthState.Dead && isAlive)
        {
            isAlive = false;
            agent.isStopped = true;
            
            // ֹͣ��������
            if (breatheComponent != null)
            {
                breatheComponent.SetBreathing(false);
            }
        }
        else if (character.HealthState != HealthState.Dead && !isAlive)
        {
            isAlive = true;
            
            // �ָ���������
            if (breatheComponent != null)
            {
                breatheComponent.SetBreathing(true);
            }
        }
    }

    void ChooseNextDestination()
    {
        if (isFocused) return; 

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void UpdateSpriteDirection(Vector3 velocity)
    {
        if (breatheComponent == null) return;

        if (velocity.x > 0.1f)  // ������
        {
            breatheComponent.SetFlipDirection(-1);  // ��ת
        }
        else if (velocity.x < -0.1f) // ������
        {
            breatheComponent.SetFlipDirection(1);   // ����
        }
    }

    IEnumerator WaitAndMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        isWaiting = false;
        ChooseNextDestination();
    }

    public void SetFocus(bool focus)
    {
        isFocused = focus;

        if (focus)
        {
            agent.isStopped = true; // ֹͣ�ƶ�
        }
        else
        {
            agent.isStopped = false; // �ָ��ƶ�
            ChooseNextDestination();
        }
    }
}
