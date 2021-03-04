using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampFall : MonoBehaviour
{
    [SerializeField] GameObject brokenLamp;
    // Start is called before the first frame update
    void Start()
    {
        brokenLamp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Golpe()
    {
        Skeleton.hitted = true;
    }
    public void Rompe()
    {
        gameObject.SetActive(false);
        brokenLamp.SetActive(true);
    }
}
