using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("BackToMain", 5);
    }
    private void BackToMain()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
