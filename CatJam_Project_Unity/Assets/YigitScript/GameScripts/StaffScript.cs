using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRadius = 5f; // Security camera detection radius
    MotherSpawn mother;
    void Start()
    {
        mother = FindObjectOfType<MotherSpawn>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            {
                mother.MotherClue();
                break;
            }
        }

    }
}
