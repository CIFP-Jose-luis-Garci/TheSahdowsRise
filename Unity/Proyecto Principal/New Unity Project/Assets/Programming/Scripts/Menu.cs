using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void Continue()
    {
        SceneManager.LoadScene("Game");
    }
    public void Options()
    {
        SceneManager.LoadScene("Options");
    }
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
