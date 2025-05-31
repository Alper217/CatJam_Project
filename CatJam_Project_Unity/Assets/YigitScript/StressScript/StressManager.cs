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
        }
    }

    public void IncreaseStress()
    { 
        if (Input.GetKeyDown(KeyCode.E) && buttonPanel.activeInHierarchy)
        {
            stressLevel = stressLevel + stressIncreaseRate;
            buttonPanel.SetActive(false);
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
}