using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] GameObject mainImage;
    private RectTransform mainImageTR;
    [SerializeField] GameObject[] controls;
    private int currentCtrl = 0;
    private int timeShown = 3;
    [HideInInspector] public bool crouchCtrl = false;
    [HideInInspector] public bool dashCtrl = false;
    [HideInInspector] public bool interactCtrl = false;
    private PlayerScript player;
    private bool shownMoveCtrl = false;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        mainImageTR = mainImage.GetComponent<RectTransform>();
        mainImage.SetActive(false);
        foreach (GameObject control in controls)
        {
            controls[currentCtrl].SetActive(false);
            currentCtrl += 1;
        }
    }
    void Update()
    {
        if (player.awaken == true && !shownMoveCtrl)
        {
            currentCtrl = 0;
            mainImage.SetActive(true);
            controls[currentCtrl].SetActive(true);
            Invoke("JumpControl", timeShown);
            shownMoveCtrl = true;
        }
        if (crouchCtrl == true)
        {
            StopCoroutine(StopCurrentCtrl());
            if (controls[1].activeInHierarchy == false)
            {
                currentCtrl = 2;
                StartCoroutine(StopCurrentCtrl());
            }
            else
            {
                Invoke("StopCrouchCtrl", timeShown);
            }
            controls[1].SetActive(false);
            mainImage.SetActive(true);
            controls[2].SetActive(true); 
            StartCoroutine(StopCurrentCtrl());
            crouchCtrl = false;
        }
        if (dashCtrl == true)
        {
            currentCtrl = 2;
            StopCoroutine(StopCurrentCtrl());
            if (controls[2].activeInHierarchy == false)
            {
                currentCtrl = 3;
                StartCoroutine(StopCurrentCtrl());
            }
            else
            {
                Invoke("StopDashCtrl", timeShown);
            }
            controls[2].SetActive(false);
            mainImage.SetActive(true);
            controls[3].SetActive(true);
            dashCtrl = false;
        }
        if (interactCtrl == true)
        {
            currentCtrl = 4;
            mainImage.SetActive(true);
            mainImageTR.position = mainImageTR.position + new Vector3(0.0f, -344.0f, 0.0f);
            controls[currentCtrl].SetActive(true);
            StartCoroutine(StopCurrentCtrl());
            interactCtrl = false;
        }
    }
    void JumpControl()
    {
        controls[currentCtrl].SetActive(false);
        currentCtrl = 1;
        controls[currentCtrl].SetActive(true);
        StartCoroutine(StopCurrentCtrl());
    }
    IEnumerator StopCurrentCtrl()
    {
        yield return new WaitForSeconds(timeShown);
        if (controls[currentCtrl + 1].activeInHierarchy == false)
        {
            mainImage.SetActive(false);
        }
        controls[currentCtrl].SetActive(false);
    }
    void StopCrouchCtrl()
    {
        mainImage.SetActive(false);
        controls[2].SetActive(false);
    }
    void StopDashCtrl()
    {
        mainImage.SetActive(false);
        controls[3].SetActive(false);
    }
}
