using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressManager : MonoBehaviour
{
    public float stressLevel = 0f;
    [SerializeField] private float stressIncreaseRate = 1f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private GameObject buttonPanel;
    public bool isOpen = true;

    private List<NPCHighlight> currentHighlightedNPCs = new List<NPCHighlight>();
    private NPCAI currentSelectedNPC; // Seçili NPC referansý

    void Start()
    {
        buttonPanel.SetActive(false);
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
        HandleInput();
    }

    public void CheckNPC()
    {
        if (isOpen)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, npcLayer);
            bool npcFound = false;
            NPCHighlight closestNPC = null;
            NPCAI closestNPCAI = null;
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
                        NPCAI npcAI = hitCollider.GetComponent<NPCAI>();

                        if (npcHighlight != null && npcAI != null)
                        {
                            closestNPC = npcHighlight;
                            closestNPCAI = npcAI;
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
                currentSelectedNPC = closestNPCAI; // Seçili NPC'yi kaydet
            }
            else
            {
                currentSelectedNPC = null;
            }

            // Panel kontrolü
            if (npcFound)
            {
                if (!buttonPanel.activeInHierarchy)
                {
                    buttonPanel.SetActive(true);
                    Debug.Log("NPC detected - Panel açýldý");
                }
            }
            else
            {
                if (buttonPanel.activeInHierarchy)
                {
                    buttonPanel.SetActive(false);
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
            currentSelectedNPC = null;
        }
    }

    private void HandleInput()
    {
        // E tuþu ile etkileþim
        if (Input.GetKeyDown(KeyCode.E) && buttonPanel.activeInHierarchy && currentSelectedNPC != null)
        {
            InteractWithNPC();
        }

        // Mouse týklama ile etkileþim (opsiyonel)
        if (Input.GetMouseButtonDown(0) && currentSelectedNPC != null && buttonPanel.activeInHierarchy)
        {
            // Raycast ile NPC'ye týklanýp týklanmadýðýný kontrol et
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, npcLayer))
            {
                if (hit.collider.gameObject == currentSelectedNPC.gameObject)
                {
                    InteractWithNPC();
                }
            }
        }
    }

    private void InteractWithNPC()
    {
        if (currentSelectedNPC != null)
        {
            // NPC'nin hareketini durdur
            currentSelectedNPC.SetWandering(false);

            // Stress seviyesini artýr
            stressLevel += stressIncreaseRate;
            Debug.Log($"NPC durduruldu! Stress Level: {stressLevel}");

            // Panel'i kapat
            buttonPanel.SetActive(false);

            // Highlight'ý kaldýr
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();

            // Belirli bir süre sonra NPC'yi tekrar hareket ettir (opsiyonel)
            StartCoroutine(ResumeNPCMovement(currentSelectedNPC, 3f));
        }
    }

    // NPC'yi belirli süre sonra tekrar hareket ettiren coroutine
    private IEnumerator ResumeNPCMovement(NPCAI npc, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (npc != null)
        {
            npc.SetWandering(true);
            Debug.Log("NPC tekrar hareket ediyor!");
        }
    }

    // Manuel olarak NPC'yi yeniden baþlatma fonksiyonu
    public void ResumeAllNPCs()
    {
        NPCAI[] allNPCs = FindObjectsOfType<NPCAI>();
        foreach (var npc in allNPCs)
        {
            npc.SetWandering(true);
        }
        Debug.Log("Tüm NPC'ler yeniden baþlatýldý!");
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
}