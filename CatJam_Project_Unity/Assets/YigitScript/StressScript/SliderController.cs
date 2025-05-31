using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] StressManager stressManager;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject goodFace;
    [SerializeField] private GameObject normalFace;
    [SerializeField] private GameObject badFace;
    [SerializeField] private GameObject worstFace;
    void Start()
    {
        slider.value = 0;
        goodFace.SetActive(true);
        normalFace.SetActive(false);
        badFace.SetActive(false);
        worstFace.SetActive(false);

    }
    void Update()
    {
        slider.value = stressManager.stressLevel;
        if (stressManager.stressLevel <=3)
        {
            goodFace.SetActive(true);
            normalFace.SetActive(false);
            badFace.SetActive(false);
            worstFace.SetActive(false);
        }
        else if (3 < stressManager.stressLevel && stressManager.stressLevel <= 6)
        {
            goodFace.SetActive(false);
            normalFace.SetActive(true);
            badFace.SetActive(false);
            worstFace.SetActive(false);
        }
        else if (6 < stressManager.stressLevel && stressManager.stressLevel <=9)
        {
            goodFace.SetActive(false);
            normalFace.SetActive(false);
            badFace.SetActive(true);
            worstFace.SetActive(false);
        }
        else
        {
            goodFace.SetActive(false);
            normalFace.SetActive(false);
            badFace.SetActive(false);
            worstFace.SetActive(true);
        }
    }
}
