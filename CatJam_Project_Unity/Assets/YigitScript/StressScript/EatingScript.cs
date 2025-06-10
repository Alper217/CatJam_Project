using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EatingScript : MonoBehaviour
{
    public static EatingScript instance;
    [SerializeField]private float stressDecreaseRate = 2f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask foodLayer;
    //[SerializeField] private GameObject interactPanel;
    //[SerializeField] private GameObject textPanel;
    public bool isEating = false;
    public bool isOpen = true;
    private AudioSource audioSource;

    StressManager stressManager;
    MoraleController moraleController;
    private List<NPCHighlight> currentHighlightedFoods = new List<NPCHighlight>();
    

    private void Awake()
    {
        EatingScript.instance = this;
        stressManager = FindObjectOfType<StressManager>();
        moraleController = FindObjectOfType<MoraleController>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        //interactPanel.SetActive(false);
        //textPanel.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    void Update()
    {
        CheckFood();
    }

    public void CheckFood()
    {
        if (isOpen)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance, foodLayer);
            bool foodFound = false;
            NPCHighlight closestFood = null;
            float closestDistance = float.MaxValue;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Food"))
                {
                    foodFound = true;
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        NPCHighlight npcHighlight = hitCollider.GetComponent<NPCHighlight>();
                        if (npcHighlight != null)
                        {
                            closestFood = npcHighlight;
                        }
                    }
                    EatFood();
                }
            }

            // Eski highlight'lar� temizle
            foreach (var food in currentHighlightedFoods)
            {
                if (food != null)
                {
                    food.SetHighlight(false);
                }
            }
            currentHighlightedFoods.Clear();

            // Sadece en yak�n NPC'yi highlight et
            if (closestFood != null)
            {
                closestFood.SetHighlight(true);
                currentHighlightedFoods.Add(closestFood);
            }

            // Panel kontrol�
            //if (foodFound)
            //{
            //    if (!interactPanel.activeInHierarchy)
            //    {
            //        interactPanel.SetActive(true);
            //        //Debug.Log("NPC detected - Panel a��ld�");
            //    }
            //}
            //else
            //{
            //    if (interactPanel.activeInHierarchy)
            //    {
            //        interactPanel.SetActive(false);
            //        //Debug.Log("No NPC detected - Panel kapat�ld�");
            //    }
            //}
        }
        else
        {
            // isOpen false ise t�m highlight'lar� kald�r
            foreach (var food in currentHighlightedFoods)
            {
                if (food != null)
                {
                    food.SetHighlight(false);
                }
            }
            currentHighlightedFoods.Clear();
        }
    }
    IEnumerator Eating()
    {
        Debug.Log("Eating started!");
        //textPanel.SetActive(true);
        //interactPanel.SetActive(false); 
        isEating = true;
        yield return new WaitForSeconds(2f);
        isEating = false;
        //textPanel.SetActive(false);
        moraleController.slider.value += 10f;
        Destroy(currentHighlightedFoods[0].gameObject); // Yeme�i yok et     
    }
    public void EatFood()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            stressManager.stressLevel -= stressDecreaseRate; // Stress Azalt/Artt�r BURAYI KONTROL ET YIGIT
            if(stressManager.stressLevel < 0)
            {
                stressManager.stressLevel = 0; // Stres seviyesi negatif olamaz
            }
            if (isEating == true)
            {
                Debug.Log("Already eating!");
                return;
            }
            if (currentHighlightedFoods.Count > 0 && currentHighlightedFoods[0] != null)
            {
                NPCHighlight higlight = currentHighlightedFoods[0].GetComponent<NPCHighlight>();
                if (higlight != null)
                {
                    StartCoroutine(Eating()); 
                }
            }

            //interactPanel.SetActive(false);
            //audioSource.Play();
        }
    }
   
    void OnDisable()
    {
        // Script devre d��� kald���nda highlight'lar� temizle
        foreach (var food in currentHighlightedFoods)
        {
            if (food != null)
            {
                food.SetHighlight(false);
            }
        }
        currentHighlightedFoods.Clear();
    }
}