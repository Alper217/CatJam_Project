using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTAManager : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 1f;
    [SerializeField] private RectTransform safeZone;
    [SerializeField] private GameObject QTAPanel;
    [SerializeField] private RectTransform pointerTransform;

    private Vector3 targetPosition;
    private bool isActive = false;

    StressManager stressManager;
    int i = 0;
    private void Awake()
    {
        stressManager = FindObjectOfType<StressManager>();
    }
    private void Start()
    {
        targetPosition = pointB.position;
        pointerTransform.position = pointA.position;
        QTAPanel.SetActive(false);
    }
    void Update()
    {
        QTAGame();
    }

    private void QTAGame()
    {

        if (stressManager.stressLevel >= 9f && i < 3)
        {
            QTAPanel.SetActive(true);
            stressManager.isOpen = false;
            isActive = true;
        }
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isActive == true)
        {
            i++;
            CheckSucces();    
            Debug.Log("Q pressed! Current count: " + i);
        }

        if (stressManager.stressLevel <= 5f)
        {
            QTAPanel.SetActive(false);
            stressManager.isOpen = true;
            isActive = false;
        }
        if (i == 3)
        {
            QTAPanel.SetActive(false);
            isActive = false;
            i = 0; 
        }
    }

    private void CheckSucces()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            stressManager.stressLevel = stressManager.stressLevel - 1f;
            Reset();
        }
        else
        {

        }
    }

    private void Reset()
    {
        targetPosition = pointB.position;
        pointerTransform.position = pointA.position;
    }
}
