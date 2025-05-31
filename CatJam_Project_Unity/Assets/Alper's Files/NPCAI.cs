using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCAI : MonoBehaviour
{
    [Header("NPC Ayarlarý")]
    public float minWalkSpeed = 1.0f;
    public float maxWalkSpeed = 3.0f;
    public float minWaitTime = 2.0f;
    public float maxWaitTime = 5.0f;
    public float wanderRadius = 10.0f;
    public float wanderDelay = 1.5f;

    [Header("Geliþmiþ Ayarlar")]
    public float personalSpace = 2.0f; // Kiþisel alan mesafesi
    public float avoidanceRadius = 3.0f; // Kaçýnma mesafesi
    public float avoidanceStrength = 2.0f;
    public float obstacleCheckDistance = 2.0f;
    public float rotationSpeed = 120f;
    public LayerMask obstacleLayer = -1;
    public LayerMask npcLayer = -1;

    [Header("Davranýþ Ayarlarý")]
    public NPCBehavior behaviorType = NPCBehavior.Normal;
    public bool canRunWhenAvoiding = true;
    public float runSpeedMultiplier = 1.8f;

    [Header("Optimizasyon")]
    public float updateFrequency = 0.1f; // Performans için güncelleme sýklýðý

    private NavMeshAgent agent;
    private Animator animator;
    private bool isWaiting = false;
    private bool isAvoiding = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 3f;
    private Coroutine avoidanceCoroutine;
    private List<Transform> nearbyNPCs = new List<Transform>();

    // Performans optimizasyonu için
    private float lastAvoidanceCheck = 0f;
    private float avoidanceCheckInterval = 0.2f;

    // Animasyon parametreleri
    private int speedHash;
    private int isWalkingHash;

    public enum NPCBehavior
    {
        Normal,     // Normal gezinme
        Shopper,    // Alýþveriþ odaklý (daha fazla durma)
        Rusher,     // Hýzlý hareket eden
        Wanderer    // Daha az hedefli gezinme
    }

    void Start()
    {
        InitializeNPC();
        SetBehaviorParameters();
        WanderToNewLocation();
        StartCoroutine(PerformanceOptimizedUpdate());
    }

    void InitializeNPC()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Animasyon hash'leri hazýrla
        if (animator != null)
        {
            speedHash = Animator.StringToHash("Speed");
            isWalkingHash = Animator.StringToHash("IsWalking");
        }

        // Agent ayarlarý
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.radius = 0.5f;
        agent.stoppingDistance = 0.3f;

        lastPosition = transform.position;
    }

    void SetBehaviorParameters()
    {
        switch (behaviorType)
        {
            case NPCBehavior.Shopper:
                minWaitTime = 3f;
                maxWaitTime = 8f;
                minWalkSpeed = 0.8f;
                maxWalkSpeed = 1.5f;
                personalSpace = 1.5f;
                break;

            case NPCBehavior.Rusher:
                minWaitTime = 0.5f;
                maxWaitTime = 2f;
                minWalkSpeed = 2f;
                maxWalkSpeed = 3.5f;
                personalSpace = 1f;
                break;

            case NPCBehavior.Wanderer:
                minWaitTime = 4f;
                maxWaitTime = 10f;
                wanderRadius = 15f;
                personalSpace = 2.5f;
                break;
        }
    }

    void Update()
    {
        EnsureOnNavMesh();
        CheckIfStuck();

        if (isWaiting || !agent.isOnNavMesh) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitBeforeNextMove());
        }
    }

    // Performans optimizasyonu için ayrý coroutine
    IEnumerator PerformanceOptimizedUpdate()
    {
        while (true)
        {
            if (Time.time - lastAvoidanceCheck > avoidanceCheckInterval)
            {
                UpdateNearbyNPCs();
                AvoidNearbyNPCs();
                lastAvoidanceCheck = Time.time;
            }

            UpdateAnimation();
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    void UpdateNearbyNPCs()
    {
        nearbyNPCs.Clear();
        Collider[] nearby = Physics.OverlapSphere(transform.position, avoidanceRadius, npcLayer);

        foreach (var col in nearby)
        {
            if (col.gameObject != gameObject && col.CompareTag("NPC"))
            {
                nearbyNPCs.Add(col.transform);
            }
        }
    }

    void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < 0.1f && !isWaiting)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckThreshold)
            {
                //Debug.Log($"{gameObject.name} sýkýþtý, yeni rota bulunuyor...");
                ForceNewDestination();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void ForceNewDestination()
    {
        // Sýkýþma durumunda daha agresif yeni hedef bulma
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * (wanderRadius * 1.5f);
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius * 2f, NavMesh.AllAreas))
            {
                if (IsPositionSafe(hit.position))
                {
                    agent.ResetPath();
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }

        // Son çare: spawn noktasýna geri dön veya random teleport
        TeleportToSafePosition();
    }

    void TeleportToSafePosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + Vector3.up * 2f, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.ResetPath();
            WanderToNewLocation();
        }
    }

    IEnumerator WaitBeforeNextMove()
    {
        isWaiting = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);

        // Davranýþ tipine göre bekleme süresi ayarlama
        if (behaviorType == NPCBehavior.Shopper && Random.Range(0f, 1f) < 0.3f)
        {
            waitTime *= 1.5f; // Alýþveriþçiler bazen daha uzun bekler
            LookAroundWhileWaiting();
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(waitTime);
        agent.isStopped = false;

        WanderToNewLocation();
        isWaiting = false;
    }

    void LookAroundWhileWaiting()
    {
        // Beklerken etrafýna bakma animasyonu
        StartCoroutine(RandomLookAround());
    }

    IEnumerator RandomLookAround()
    {
        float lookDuration = Random.Range(1f, 3f);
        Vector3 originalForward = transform.forward;

        for (int i = 0; i < 3; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
            randomDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
            float rotTime = 0;

            while (rotTime < 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    Time.deltaTime * rotationSpeed / 60f);
                rotTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    void EnsureOnNavMesh()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = false;
                agent.enabled = true;
            }
            else
            {
                //Debug.LogWarning($"{gameObject.name} NavMesh'e geri döndürülemedi!");
            }
        }
    }

    void WanderToNewLocation()
    {
        int maxTries = 20;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 randomDirection = GetSmartRandomDirection();
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                if (IsPositionSafe(hit.position) && HasClearPath(hit.position))
                {
                    SetDestinationWithSpeed(hit.position);
                    return;
                }
            }
        }

        //Debug.LogWarning($"{gameObject.name} için güvenli hedef bulunamadý, alternatif yöntem deneniyor.");
        FindAlternativeDestination();
    }

    Vector3 GetSmartRandomDirection()
    {
        Vector3 baseDirection;

        // Bazen mevcut yönde devam et (daha doðal görünür)
        if (Random.Range(0f, 1f) < 0.4f && agent.velocity.magnitude > 0.1f)
        {
            baseDirection = agent.velocity.normalized * wanderRadius * 0.7f;
        }
        else
        {
            baseDirection = Random.insideUnitSphere * wanderRadius;
        }

        baseDirection.y = 0;
        return transform.position + baseDirection;
    }

    bool IsPositionSafe(Vector3 position)
    {
        // Engel kontrolü
        Collider[] obstacles = Physics.OverlapSphere(position, obstacleCheckDistance, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            if (obstacle.gameObject != gameObject && !obstacle.isTrigger)
            {
                return false;
            }
        }

        // NPC yoðunluðu kontrolü
        Collider[] npcs = Physics.OverlapSphere(position, personalSpace, npcLayer);
        int npcCount = 0;
        foreach (var npc in npcs)
        {
            if (npc.gameObject != gameObject && npc.CompareTag("NPC"))
            {
                npcCount++;
            }
        }

        return npcCount <= 2; // Maksimum 2 NPC yakýnda olabilir
    }

    bool HasClearPath(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, distance, obstacleLayer))
        {
            return false;
        }

        return true;
    }

    void SetDestinationWithSpeed(Vector3 destination)
    {
        float randomSpeed = Random.Range(minWalkSpeed, maxWalkSpeed);

        // Davranýþ tipine göre hýz ayarlama
        switch (behaviorType)
        {
            case NPCBehavior.Rusher:
                randomSpeed *= Random.Range(1.2f, 1.5f);
                break;
            case NPCBehavior.Shopper:
                randomSpeed *= Random.Range(0.7f, 0.9f);
                break;
        }

        agent.speed = randomSpeed;
        agent.SetDestination(destination);
    }

    void FindAlternativeDestination()
    {
        // Mevcut pozisyondan daha küçük adýmlarla hedef ara
        for (int radius = 3; radius <= wanderRadius; radius += 2)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                Vector3 direction = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * radius;

                Vector3 testPosition = transform.position + direction;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(testPosition, out hit, 2f, NavMesh.AllAreas))
                {
                    if (IsPositionSafe(hit.position))
                    {
                        SetDestinationWithSpeed(hit.position);
                        return;
                    }
                }
            }
        }
    }

    void AvoidNearbyNPCs()
    {
        if (nearbyNPCs.Count == 0) return;

        Vector3 avoidanceForce = Vector3.zero;
        int criticalAvoidanceCount = 0;

        foreach (Transform npcTransform in nearbyNPCs)
        {
            if (npcTransform == null) continue;

            float distance = Vector3.Distance(transform.position, npcTransform.position);

            if (distance < personalSpace)
            {
                Vector3 awayDirection = (transform.position - npcTransform.position).normalized;
                float forceStrength = (personalSpace - distance) / personalSpace;
                avoidanceForce += awayDirection * forceStrength;

                if (distance < personalSpace * 0.5f)
                {
                    criticalAvoidanceCount++;
                }
            }
        }

        if (avoidanceForce.magnitude > 0.1f)
        {
            ApplyAvoidanceForce(avoidanceForce, criticalAvoidanceCount > 0);
        }
    }

    void ApplyAvoidanceForce(Vector3 avoidanceForce, bool isCritical)
    {
        Vector3 avoidanceTarget = transform.position + avoidanceForce.normalized * avoidanceStrength;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(avoidanceTarget, out hit, avoidanceStrength * 2f, NavMesh.AllAreas))
        {
            if (IsPositionSafe(hit.position))
            {
                // Kritik durumda hýzý artýr
                if (isCritical && canRunWhenAvoiding)
                {
                    float oldSpeed = agent.speed;
                    agent.speed = oldSpeed * runSpeedMultiplier;

                    if (avoidanceCoroutine != null)
                        StopCoroutine(avoidanceCoroutine);

                    avoidanceCoroutine = StartCoroutine(ResetSpeedAfterAvoidance(oldSpeed, 2f));
                }

                agent.SetDestination(hit.position);
                isAvoiding = true;
            }
        }
    }

    IEnumerator ResetSpeedAfterAvoidance(float originalSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null)
        {
            agent.speed = originalSpeed;
            isAvoiding = false;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        float currentSpeed = agent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / maxWalkSpeed;

        animator.SetFloat(speedHash, normalizedSpeed);
        animator.SetBool(isWalkingHash, currentSpeed > 0.1f);
    }

    // Debug görselleþtirme
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, personalSpace);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckDistance);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            Vector3[] path = agent.path.corners;
            for (int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}