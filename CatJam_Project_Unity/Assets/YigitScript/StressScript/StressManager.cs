using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressManager : MonoBehaviour
{
    public static StressManager instance;
    public float stressLevel = 0f;
    [SerializeField] private float stressIncreaseRate = 1f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private GameObject textPanel;
    public bool isOpen = true;
    private AudioSource audioSource;

    private List<NPCHighlight> currentHighlightedNPCs = new List<NPCHighlight>();
    private void Awake()
    {
        StressManager.instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        interactPanel.SetActive(false);
        textPanel.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);
    }

    void Update()
    {
        CheckNPC();
        IncreaseStress();
    }

    public void CheckNPC()
    {
        if (isOpen)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, npcLayer);
            bool npcFound = false;
            NPCHighlight closestNPC = null;
            float closestDistance = float.MaxValue;

            // En yakýn NPC'yi bul
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("NPC"))
                {
                    npcFound = true;
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        NPCHighlight npcHighlight = hitCollider.GetComponent<NPCHighlight>();
                        if (npcHighlight != null)
                        {
                            closestNPC = npcHighlight;
                        }
                    }
                }
            }

            // Eski highlight'larý temizle
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();

            // Sadece en yakýn NPC'yi highlight et
            if (closestNPC != null)
            {
                closestNPC.SetHighlight(true);
                currentHighlightedNPCs.Add(closestNPC);
            }

            // Panel kontrolü
            if (npcFound)
            {
                if (!interactPanel.activeInHierarchy)
                {
                    interactPanel.SetActive(true);
                    Debug.Log("NPC detected - Panel açýldý");
                }
            }
            else
            {
                if (interactPanel.activeInHierarchy)
                {
                    interactPanel.SetActive(false);
                    Debug.Log("No NPC detected - Panel kapatýldý");
                }
            }
        }
        else
        {
            // isOpen false ise tüm highlight'larý kaldýr
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();
        }
    }
    IEnumerator ResumeNPCAfterDelay(NPCAI npcAI, float delay)
    {
        yield return new WaitForSeconds(delay);
        npcAI.ResumeNPC();
    }
    public void IncreaseStress()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactPanel.activeInHierarchy)
        {
            stressLevel += stressIncreaseRate;

            if (currentHighlightedNPCs.Count > 0 && currentHighlightedNPCs[0] != null)
            {
                NPCAI npcAI = currentHighlightedNPCs[0].GetComponent<NPCAI>();
                if (npcAI != null)
                {
                    npcAI.StopNPC(); // Hemen durdur
                    StartCoroutine(ResumeNPCAfterDelay(npcAI, 3f)); // 3 saniye sonra tekrar baþlasýn
                }
            }

            interactPanel.SetActive(false);
            audioSource.Play();
            StartCoroutine(HideTextPanel());
        }
    }
    void OnDisable()
    {
        // Script devre dýþý kaldýðýnda highlight'larý temizle
        foreach (var npc in currentHighlightedNPCs)
        {
            if (npc != null)
            {
                npc.SetHighlight(false);
            }
        }
        currentHighlightedNPCs.Clear();
    }

    IEnumerator HideTextPanel()
    {
        textPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        textPanel.SetActive(false);
    }
}