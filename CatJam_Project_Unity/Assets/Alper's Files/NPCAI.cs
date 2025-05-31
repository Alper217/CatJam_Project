using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class NPCAI : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float speed = 3.5f;

    [Header("Durum")]
    public bool isWandering = true;
    public bool isStopped = false; // Durdurulma durumu

    private NavMeshAgent agent;
    private float timer;
    private Vector3 startPosition;
    private Animator animator;
    private bool wasMoving = false;
    [SerializeField] Transform characterTransform;

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

        // Baþlangýçta idle animasyon
        UpdateAnimation(false);
    }

    void Update()
    {
        if (!isWandering || agent == null || isStopped)
        {
            if (isStopped)
            {
                UpdateAnimation(false); // Durdurulduðunda idle animasyon
            }
            return;
        }

        timer += Time.deltaTime;

        // Hareket durumunu kontrol et ve animasyonu güncelle
        bool isMoving = agent.velocity.magnitude > 0.1f;
        if (isMoving != wasMoving)
        {
            UpdateAnimation(isMoving);
            wasMoving = isMoving;
        }

        // Hedefe ulaþtý veya zaman doldu
        if (timer >= wanderTimer || !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GetNewDestination();
            timer = 0f;
        }
    }

    void UpdateAnimation(bool moving)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", moving);
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
        isStopped = !wander; // Wandering kapatýldýðýnda durdur

        if (!wander)
        {
            agent.ResetPath();
            agent.isStopped = true;
            UpdateAnimation(false); // Idle animasyon
            Debug.Log($"{gameObject.name} durduruldu!");
            if (characterTransform != null)
            {
                Vector3 direction = (characterTransform.position - transform.position).normalized;
                direction.y = 0; // Yukarý aþaðý bakmasýný istemiyorsak
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            agent.isStopped = false;
            Debug.Log($"{gameObject.name} tekrar hareket ediyor!");
        }
    }

    public void StopNPC()
    {
        SetWandering(false);
    }

    public void ResumeNPC()
    {
        SetWandering(true);
    }

    public void GoToPosition(Vector3 targetPos)
    {
        if (agent != null)
        {
            isStopped = false;
            agent.isStopped = false;
            agent.SetDestination(targetPos);
            isWandering = false;
            UpdateAnimation(true);
        }
    }

    public void ReturnToStart()
    {
        GoToPosition(startPosition);
    }

    // NPC'nin durumunu kontrol etmek için
    public bool IsMoving()
    {
        return agent != null && agent.velocity.magnitude > 0.1f && !isStopped;
    }

    public bool IsStopped()
    {
        return isStopped;   
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isStopped ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(startPosition, wanderRadius);

        if (agent != null && agent.hasPath && !isStopped)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}