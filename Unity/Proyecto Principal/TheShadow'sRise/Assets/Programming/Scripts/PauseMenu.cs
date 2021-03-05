using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pause;
    private bool paused = false;
    [SerializeField] Button continueButton;
    [HideInInspector] public static bool canJump;
    [HideInInspector] public static bool fromGame;
    // Start is called before the first frame update
    void Start()
    {
        pause.SetActive(false);
        canJump = true;
        fromGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape) && paused == true)
        if (Input.GetButtonDown("Pausa") && paused == true)
        {
            paused = false;
        }
        //else if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape) && paused == false)
        else if (Input.GetButtonDown("Pausa") && paused == false)
        {
            pause.SetActive(true);
            Time.timeScale = 0;
            paused = true;
            AudioListener.pause = true;
            continueButton.Select();
        }
        if (paused == false)
        {
            Time.timeScale = 1;
            pause.SetActive(false);
            AudioListener.pause = false;
            Invoke("NotJump", 0.1f);
            EventSystem.current.SetSelectedGameObject(null);
        }
        if (pause.activeInHierarchy == true)
        {
            canJump = false;
        }
    }
    public void Continue()
    {
        paused = false;
    }
    public void Menu()
    {
        SceneManager.LoadScene("MenuPrincipal");
        fromGame = true;
    }
    public void Salir()
    {
        Application.Quit();
    }
    private void NotJump()
    {
        canJump = true;
    }
}
