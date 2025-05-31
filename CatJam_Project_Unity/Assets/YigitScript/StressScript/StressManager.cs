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
    }

    private void CheckNPC()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, npcLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("NPC"))
            {
                buttonPanel.SetActive(true);
                //Debug.Log("NPC detected");
                return;
            }
            else
            {
                buttonPanel.SetActive(false);
                Debug.Log("No NPC detected");
            }
        }

        //Ray ray;

        //ray = new Ray(transform.position, transform.forward);

        //if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, npcLayer))
        //{
        //    buttonPanel.SetActive(true);
        //    Debug.Log("Raycast hit");
        //}
        //else
        //{
        //    buttonPanel.SetActive(false);
        //}
    }

    public void IncreaseStress()
    {
        stressLevel = stressLevel + stressIncreaseRate;
    }
}
