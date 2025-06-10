using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MotherSpawn : MonoBehaviour
{
    public static MotherSpawn instance;
    [SerializeField] private List<GameObject> mothers;
    [SerializeField] private GameObject �ndex0Panel;
    [SerializeField] private GameObject �ndex1Panel;
    [SerializeField] private GameObject �ndex2Panel;
    [SerializeField] private GameObject �ndex3Panel;
    [SerializeField] private GameObject index0yer;
    [SerializeField] private GameObject index0k�yafet;
    [SerializeField] private GameObject index0sac;
    [SerializeField] private GameObject index1yer;
    [SerializeField] private GameObject index1k�yafet;
    [SerializeField] private GameObject index1sac;
    [SerializeField] private GameObject index2yer;
    [SerializeField] private GameObject index2k�yafet;
    [SerializeField] private GameObject index2sac;
    [SerializeField] private GameObject index3yer;
    [SerializeField] private GameObject index3k�yafet;
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
                �ndex0Panel.SetActive(true);
                if(i == 0 && bekle==false)
                {
                    index0yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index0k�yafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index0sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 1:
                �ndex1Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index1yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index1k�yafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index1sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 2:
                �ndex2Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index2yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index2k�yafet.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 2 && bekle == false)
                {
                    index2sac.SetActive(true);
                    StartCoroutine(Sure());
                }
                break;
            case 3:
                �ndex3Panel.SetActive(true);
                if (i == 0 && bekle == false)
                {
                    index3yer.SetActive(true);
                    StartCoroutine(Sure());
                }
                if (i == 1 && bekle == false)
                {
                    index3k�yafet.SetActive(true);
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
