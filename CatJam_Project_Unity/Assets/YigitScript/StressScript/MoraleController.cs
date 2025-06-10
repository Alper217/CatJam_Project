using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoraleController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    void Start()
    {
        slider.value = 100f;
    }
    void Update()
    {
        slider.value -= 20f*Time.deltaTime;
        Debug.Log("Düþüyor");
    }
}
