using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoraleController : MonoBehaviour
{
    [SerializeField] public Slider slider;
    PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        slider.value = 100f;
    }
    void Update()
    {
        slider.value -= 1f*Time.deltaTime;
        playerController.canRun = slider.value > 75;
       if (slider.value <= 50 )
       {
            playerController.walkSpeed = 2.5f;
       }
    }
}
