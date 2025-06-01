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
    [SerializeField] private float qtaCooldown = 5f; // QTA aras�ndaki bekleme s�resi

    private Vector3 targetPosition;
    private bool isActive = false;

    StressManager stressManager;
    int i = 0;
    private bool qtaSessionActive = false; // QTA oturumunun aktif olup olmad���n� takip eder
    private float lastQTAEndTime = 0f; // Son QTA'n�n bitti�i zaman

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
        // QTA paneli a�ma ko�ulu - stress y�ksek oldu�unda, �u anda aktif de�ilse ve cooldown s�resi ge�mi�se
        if (stressManager.stressLevel == 9f && !qtaSessionActive && Time.time - lastQTAEndTime >= qtaCooldown)
        {
            StartQTASession();
        }

        // Sadece QTA aktifken pointer hareket etsin
        if (qtaSessionActive)
        {
            pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
            {
                targetPosition = pointB.position;
            }
            else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
            {
                Reset();
            }

            // Q tu�una basma kontrol�
            if (Input.GetKeyDown(KeyCode.Space) && isActive)
            {
                CheckSucces();
                i++;
                Debug.Log("Q pressed! Current count: " + i);
            }

            // Stress d��t���nde QTA'y� sonland�r
            if (stressManager.stressLevel <= 5f)
            {
                EndQTASession();
            }

            // 3 deneme sonras� QTA'y� sonland�r (ba�ar�l� ya da ba�ar�s�z)
            if (i >= 3)
            {
                EndQTASession();
            }
        }
    }

    private void StartQTASession()
    {
        QTAPanel.SetActive(true);
        stressManager.isOpen = false;
        isActive = true;
        qtaSessionActive = true;
        i = 0; // Sayac� s�f�rla
        Reset(); // Pointer pozisyonunu s�f�rla
        Debug.Log("QTA Session Started");
    }

    private void EndQTASession()
    {
        QTAPanel.SetActive(false);
        stressManager.isOpen = true;
        isActive = false;
        qtaSessionActive = false;
        lastQTAEndTime = Time.time; // Son QTA'n�n bitti�i zaman� kaydet
        i = 0; // Sayac� s�f�rla
        Debug.Log("QTA Session Ended - Next QTA available in " + qtaCooldown + " seconds");
    }

    private void CheckSucces()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            stressManager.stressLevel = stressManager.stressLevel - 1f;
            Reset();
            Debug.Log("Success! Stress reduced. Current stress: " + stressManager.stressLevel);
        }
        else
        {
            // Ba�ar�s�z oldu�unda da stres'i hafif�e azalt (sonsuz d�ng�y� �nlemek i�in)
            stressManager.stressLevel = stressManager.stressLevel - 0.3f;
            Reset();
            Debug.Log("Failed! Pointer not in safe zone. Stress slightly reduced: " + stressManager.stressLevel);
        }
    }

    private void Reset()
    {
        targetPosition = pointB.position;
        pointerTransform.position = pointA.position;
    }
}