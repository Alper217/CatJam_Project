using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherSpawn : MonoBehaviour
{
    public static MotherSpawn instance;
    [SerializeField] private List<GameObject> mothers;
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
                Debug.Log(motherIndex); 
                break;
            case 1:
                Debug.Log(motherIndex);
                break;
            case 2:
                Debug.Log(motherIndex);
                break;
            case 3:
                Debug.Log(motherIndex);
                break;
        }
    }
}
