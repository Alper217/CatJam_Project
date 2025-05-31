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
                        if (npcHighlight != null)
                        {
                            closestNPC = npcHighlight;
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