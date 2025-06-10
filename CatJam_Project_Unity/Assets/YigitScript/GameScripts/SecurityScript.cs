using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private GameObject gameOverPanel;
    EatingScript eatingScript;
    void Start()
    {
        eatingScript = FindObjectOfType<EatingScript>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,detectionRadius);
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
     
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && eatingScript.isEating == true)
            {
               // gameOverPanel.SetActive(true);
                Debug.Log("Player detected by security camera");
                break;
            }
        }

    }
}
