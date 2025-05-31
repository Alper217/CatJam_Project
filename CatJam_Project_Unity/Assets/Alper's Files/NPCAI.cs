using UnityEngine;
using UnityEngine.AI;

public class NPCAI : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float speed = 3.5f;

    [Header("Durum")]
    public bool isWandering = true;

    private NavMeshAgent agent;
    private float timer;
    private Vector3 startPosition;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component bulunamadý!");
            return;
        }

        agent.speed = speed;
        startPosition = transform.position;
        timer = wanderTimer;
    }

    void Update()
    {
        if (!isWandering || agent == null) return;
        animator.SetBool("isMoving", true);
        timer += Time.deltaTime;
        if (timer >= wanderTimer || !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GetNewDestination();
            timer = 0f;
        }
    }

    void GetNewDestination()
    {
        Vector3 newPos = GetRandomNavMeshPosition();
        agent.SetDestination(newPos);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += startPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            return hit.position;
        }
        return transform.position;
    }

    public void SetWandering(bool wander)
    {
        isWandering = wander;
        if (!wander)
        {
            agent.ResetPath();
        }
    }

    public void GoToPosition(Vector3 targetPos)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPos);
            isWandering = false;
        }
    }

    public void ReturnToStart()
    {
        GoToPosition(startPosition);   
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startPosition, wanderRadius);
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}