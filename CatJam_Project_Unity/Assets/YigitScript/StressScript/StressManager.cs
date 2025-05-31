using UnityEngine;

public class StressManager : MonoBehaviour
{
    [SerializeField] private float stressLevel = 0f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask npcLayer;    
    [SerializeField] private float stressIncreaseRate = 1f;
    [SerializeField] private GameObject buttonPanel;
    

    void Start()
    {
        buttonPanel.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, maxDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);
    }

    void Update()
    {
        CheckNPC();
    }
    
    private void CheckNPC()
    {
        //Ray ray;
        //ray = new Ray(transform.position, transform.forward);

        //if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, npcLayer))
        //{
        //    OpenButton();
        //}

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, npcLayer);
        //foreach (var hitCollider in hitColliders)
        //{
        //    if (hitCollider.CompareTag("NPC"))
        //    {
        //        // Open the button panel when an NPC is detected
        //        OpenButton();
        //        return; // Exit after the first NPC is found
        //    }
            
        //}
    }
    private void OpenButton()
    {
        buttonPanel.SetActive(true);
    }
    public void IncreaseStress()
    {
        stressLevel += stressIncreaseRate * Time.deltaTime;
        Debug.Log("Stress Level: " + stressLevel);
    }

}

