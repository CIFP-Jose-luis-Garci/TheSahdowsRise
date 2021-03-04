using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArrowPosition : MonoBehaviour
{
    [SerializeField] Transform arrow;
    [SerializeField] Transform newGame;
    [SerializeField] Transform contGame;
    [SerializeField] Transform options;
    [SerializeField] Transform credits;
    [SerializeField] Transform exit;
    private AsyncOperation loadingOperation;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject smoke0;
    [SerializeField] GameObject smoke1;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider progressBar;
    private void Start()
    {
        loadingScreen.SetActive(false);
    }
    private void Update()
    {
        if (loadingScreen.activeInHierarchy == true)
        {
            progressBar.value = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        }
    }
    public void NewGame()
    {
        arrow.position = newGame.position;
    }
    public void ContinueGame()
    {
        arrow.position = contGame.position;
    }
    public void Options()
    {
        arrow.position = options.position;
    }
    public void Credits()
    {
        arrow.position = credits.position;
    }
    public void Exit()
    {
        arrow.position = exit.position;
    }
    public void StartGame()
    {
        Invoke("StartGameReal", 2);
        //loadingOperation = SceneManager.LoadSceneAsync("DEFINITIVA");
        canvas.SetActive(false);
        smoke0.SetActive(false);
        smoke1.SetActive(false);
        loadingScreen.SetActive(true);
    }
    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options");
    }
    public void SeeCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void StartGameReal()
    {
        loadingOperation = SceneManager.LoadSceneAsync("DEFINITIVA");
    }
}
