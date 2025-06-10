using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MotherSpawn : MonoBehaviour
{
    public static MotherSpawn instance;
    [SerializeField] private List<GameObject> mothers;
    [SerializeField] private GameObject ýndex0Panel;
    [SerializeField] private GameObject ýndex1Panel;
    [SerializeField] private GameObject ýndex2Panel;
    [SerializeField] private GameObject ýndex3Panel;
    [SerializeField] private GameObject index0yer;
    [SerializeField] private GameObject index0kýyafet;
    [SerializeField] private GameObject index0sac;
    [SerializeField] private GameObject index1yer;
    [SerializeField] private GameObject index1kýyafet;
    [SerializeField] private GameObject index1sac;
    [SerializeField] private GameObject index2yer;
    [SerializeField] private GameObject index2kýyafet;
    [SerializeField] private GameObject index2sac;
    [SerializeField] private GameObject index3yer;
    [SerializeField] private GameObject index3kýyafet;
    [SerializeField] private GameObject index3sac;
    private int i = 0;
    private bool bekle = false;
    int motherIndex;
    private void Awake()
    {
        MotherSpawn.instance = this;
    }
    void Start()
    {
        if (mothers.Count > 0)
        {
            motherIndex = Random.Range(0, mothers.Count);
            mothers[motherIndex].tag = "MOM";
            Debug.Log(motherIndex);
        }
        else
        {
            Debug.LogWarning("No mothers available to spawn.");
        }
    }

    public void MotherClue()
    {
        
        switch (motherIndex)
        {
            case 0:
                ýndex0Panel.SetActive(true);
                if(i == 0 && bekle==false)
                {
                    index0yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index0kýyafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index0sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 1:
                ýndex1Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index1yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index1kýyafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index1sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 2:
                ýndex2Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index2yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index2kýyafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index2sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 3:
                ýndex3Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index3yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index3kýyafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index3sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
        }
  
    }

    IEnumerator Sure()
    {
        bekle = true;
        yield return new WaitForSeconds(2f);
        i++;
        bekle = false;
    }
}
