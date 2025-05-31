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
    private NPCAI currentSelectedNPC; // Se�ili NPC referans�

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

            // En yak�n NPC'yi bul
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

            // Eski highlight'lar� temizle
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();

            // Sadece en yak�n NPC'yi highlight et
            if (closestNPC != null)
            {
                closestNPC.SetHighlight(true);
                currentHighlightedNPCs.Add(closestNPC);
                currentSelectedNPC = closestNPCAI; // Se�ili NPC'yi kaydet
            }
            else
            {
                currentSelectedNPC = null;
            }

            // Panel kontrol�
            if (npcFound)
            {
                if (!buttonPanel.activeInHierarchy)
                {
                    buttonPanel.SetActive(true);
                    Debug.Log("NPC detected - Panel a��ld�");
                }
            }
            else
            {
                if (buttonPanel.activeInHierarchy)
                {
                    buttonPanel.SetActive(false);
                    Debug.Log("No NPC detected - Panel kapat�ld�");
                }
            }
        }
        else
        {
            // isOpen false ise t�m highlight'lar� kald�r
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
        // E tu�u ile etkile�im
        if (Input.GetKeyDown(KeyCode.E) && buttonPanel.activeInHierarchy && currentSelectedNPC != null)
        {
            InteractWithNPC();
        }

        // Mouse t�klama ile etkile�im (opsiyonel)
        if (Input.GetMouseButtonDown(0) && currentSelectedNPC != null && buttonPanel.activeInHierarchy)
        {
            // Raycast ile NPC'ye t�klan�p t�klanmad���n� kontrol et
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

            // Stress seviyesini art�r
            stressLevel += stressIncreaseRate;
            Debug.Log($"NPC durduruldu! Stress Level: {stressLevel}");

            // Panel'i kapat
            buttonPanel.SetActive(false);

            // Highlight'� kald�r
            foreach (var npc in currentHighlightedNPCs)
            {
                if (npc != null)
                {
                    npc.SetHighlight(false);
                }
            }
            currentHighlightedNPCs.Clear();

            // Belirli bir s�re sonra NPC'yi tekrar hareket ettir (opsiyonel)
            StartCoroutine(ResumeNPCMovement(currentSelectedNPC, 3f));
        }
    }

    // NPC'yi belirli s�re sonra tekrar hareket ettiren coroutine
    private IEnumerator ResumeNPCMovement(NPCAI npc, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (npc != null)
        {
            npc.SetWandering(true);
            Debug.Log("NPC tekrar hareket ediyor!");
        }
    }

    // Manuel olarak NPC'yi yeniden ba�latma fonksiyonu
    public void ResumeAllNPCs()
    {
        NPCAI[] allNPCs = FindObjectsOfType<NPCAI>();
        foreach (var npc in allNPCs)
        {
            npc.SetWandering(true);
        }
        Debug.Log("T�m NPC'ler yeniden ba�lat�ld�!");
    }

    void OnDisable()
    {
        // Script devre d��� kald���nda highlight'lar� temizle
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