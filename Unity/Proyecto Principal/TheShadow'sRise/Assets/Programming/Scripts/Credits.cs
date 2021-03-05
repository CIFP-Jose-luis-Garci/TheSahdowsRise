using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private float speed = 5f;
    private float despl;
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        despl = speed * Time.deltaTime;
    }
    void Update()
    {
        rectTransform.Translate(0.0f, despl, 0.0f);
        if (rectTransform.localPosition.y >= 1500.0f)
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
    public void Back()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
