using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CharacterMoveAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isWaiting = false;
    private Vector3 baseScale;

    public float walkRadius = 5f;     // ��뾶
    public float minWait = 2f;        // �ȴ�ʱ�䷶Χ
    public float maxWait = 5f;
    public float breatheSpeed = 2f;   // �����ٶ�
    public float breatheScale = 0.05f;// ��������

    private bool isFocused = false;  

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseScale = transform.localScale;
        ChooseNextDestination();
    }

    void Update()
    {
        if (!isFocused) 
        {
            // ����Ŀ�� �� �ȴ�
            if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.2f)
            {
                StartCoroutine(WaitAndMove());
            }
        }

        // �����������۽�ʱҲ�к�����
        float breathe = Mathf.Sin(Time.time * breatheSpeed) * breatheScale;
        transform.localScale = baseScale + new Vector3(0, breathe, 0);

        if (!isFocused)  
        {
            UpdateSpriteDirection(agent.velocity);
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
        if (velocity.x > 0.1f)  // ������
            transform.localScale = new Vector3(1, transform.localScale.y, 1);
        else if (velocity.x < -0.1f) // ������
            transform.localScale = new Vector3(-1, transform.localScale.y, 1);
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
