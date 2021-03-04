using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private Image image;
    [SerializeField] Sprite[] spriteArray;
    public static bool gotCoin = false;
    void Start()
    {
        image = GetComponent<Image>();
    }
    void Update()
    {
        if (gotCoin == true)
        {
            image.sprite = spriteArray[0];
        }
        else if (gotCoin == false)
        {
            image.sprite = spriteArray[1];
        }
    }
}
