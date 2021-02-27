using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private bool isLooking = false;
    private bool isMoving;
    private bool hittable = false;
    private float speed = 2f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] GameObject[] flames;
    [SerializeField] GameObject[] flamesParent;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        animator.SetBool("Moviendo", isMoving);
        if (!isLooking)
        {
            Pathing();
        }
        FlamesSprites();
    }
    void Pathing()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        isMoving = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Limit")
        {
            speed = speed * -1;
            if (spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }
            else if (spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GuardPoint")
        {
            StartCoroutine(Guarding());
        }
    }
    IEnumerator Guarding()
    {
        isLooking = true;
        animator.SetBool("Vigilando", isLooking);
        hittable = true;
        yield return new WaitForSeconds(2);
        hittable = false;
        yield return new WaitForSeconds(1);
        isLooking = false;
        animator.SetBool("Vigilando", isLooking);
        StopCoroutine(Guarding());
    }
    private void FlamesSprites()
    {
        if (!spriteRenderer.flipX)
        {
            flamesParent[0].SetActive(true);
            flamesParent[1].SetActive(false);
            if (!isMoving && !isLooking)
            {
                flames[0].SetActive(true);
                flames[1].SetActive(false);
                flames[2].SetActive(false);
                flames[3].SetActive(false);
            }
            else if (isMoving && !isLooking)
            {
                flames[0].SetActive(false);
                flames[1].SetActive(false);
                flames[2].SetActive(false);
                flames[3].SetActive(true);
            }
            else if (isMoving && isLooking)
            {
                flames[0].SetActive(false);
                flames[1].SetActive(true);
                flames[2].SetActive(false);
                flames[3].SetActive(false);
            }
        }
        else if (spriteRenderer.flipX)
        {
            flamesParent[0].SetActive(false);
            flamesParent[1].SetActive(true);
            if (!isMoving && !isLooking)
            {
                flames[4].SetActive(true);
                flames[5].SetActive(false);
                flames[6].SetActive(false);
                flames[7].SetActive(false);
            }
            else if (isMoving && !isLooking)
            {
                flames[4].SetActive(false);
                flames[5].SetActive(false);
                flames[6].SetActive(false);
                flames[7].SetActive(true);
            }
            else if (isMoving && isLooking)
            {
                flames[4].SetActive(false);
                flames[5].SetActive(true);
                flames[6].SetActive(false);
                flames[7].SetActive(false);
            }
        }
    }
}
