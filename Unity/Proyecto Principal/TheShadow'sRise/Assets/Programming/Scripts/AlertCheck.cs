using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertCheck : MonoBehaviour
{
    private Image image;
    [SerializeField] Sprite[] spriteList;
    void Start()
    {
        image = GetComponent<Image>();
    }
    void Update()
    {
        if (!Skeleton.hitted)
        {
            if (Detector.almostSpotted == true && !Detector.spotted)
            {
                image.sprite = spriteList[1];
            }
            else if (Detector.spotted == true)
            {
                image.sprite = spriteList[2];
            }
            else if (!Detector.spotted && !Detector.almostSpotted)
            {
                image.sprite = spriteList[0];
            }
        }
    }
}
