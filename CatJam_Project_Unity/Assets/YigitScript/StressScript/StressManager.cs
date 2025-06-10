using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StressManager : MonoBehaviour
{
    public static StressManager instance;
    MotherSpawn motherSpawn;
    public float stressLevel = 0f;
    [SerializeField] private float stressIncreaseRate = 1f;
    [SerializeField] private float maxStress = 12f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    public bool isOpen = true;
    private AudioSource audioSource;

    private List<NPCHighlight> currentHighlightedNPCs = new List<NPCHighlight>();
    private void Awake()
    {
        StressManager.instance = this;
        motherSpawn = FindObjectOfType<MotherSpawn>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        interactPanel.SetActive(false);
        textPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    void Update()
    {
        CheckNPC();
        
        GameOver();
    }

    public void CheckNPC()
    {
        if (isOpen)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, npcLayer);
            bool npcFound = false;
            NPCHighlight closestNPC = null;
            float closestDistance = float.MaxValue;

            // En yakın NPC'yi bul
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
                    IncreaseStress();
                }
                if (hitCollider.CompareTag("MOM"))
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
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        winPanel.SetActive(true);
                        Time.timeScale = 0f; // Oyun durdurulsun

                        //Debug.Log("MOM detected - Scene changed to MOM scene");
                    }
                    
                }
                
            }

            // Eski highlight'ları temizle
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();

            // Sadece en yakın NPC'yi highlight et
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
                    //Debug.Log("NPC detected - Panel açıldı");
                }
            }
            else
            {
                if (interactPanel.activeInHierarchy)
                {
                    interactPanel.SetActive(false);
                    //Debug.Log("No NPC detected - Panel kapatıldı");
                }
            }
        }
        else
        {
            // isOpen false ise tüm highlight'ları kaldır
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
                    StartCoroutine(ResumeNPCAfterDelay(npcAI, 3f)); // 3 saniye sonra tekrar başlasın
                }
            }

            interactPanel.SetActive(false);
            audioSource.Play();
            StartCoroutine(HideTextPanel());
        }
    }
    private void GameOver()
    {
        if(stressLevel >= maxStress)
        {
            Debug.Log("Game Over - Stress Level exceeded");
            gameOverPanel.SetActive(true);
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);  
    }
    void OnDisable()
    {
        // Script devre dışı kaldığında highlight'ları temizle
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