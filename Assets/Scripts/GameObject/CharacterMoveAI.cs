using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CharacterMoveAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterData character;
    private bool isAlive = true;
    private bool isWaiting = false;
    [SerializeField] private CharacterBreatheComponent breatheComponent;

    public float walkRadius = 5f;     // 活动半径
    public float minWait = 2f;        // 等待时间范围
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
            // 到达目标 → 等待
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
            
            // 停止呼吸动画
            if (breatheComponent != null)
            {
                breatheComponent.SetBreathing(false);
            }
        }
        else if (character.HealthState != HealthState.Dead && !isAlive)
        {
            isAlive = true;
            
            // 恢复呼吸动画
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

        if (velocity.x > 0.1f)  // 往右走
        {
            breatheComponent.SetFlipDirection(-1);  // 翻转
        }
        else if (velocity.x < -0.1f) // 往左走
        {
            breatheComponent.SetFlipDirection(1);   // 正常
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
            agent.isStopped = true; // 停止移动
        }
        else
        {
            agent.isStopped = false; // 恢复移动
            ChooseNextDestination();
        }
    }

    public void MoveToIfValid(Vector3 targetPos)
    {
        if (!isAlive || isFocused) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
            //isWaiting = false; 
        }
    }
}
