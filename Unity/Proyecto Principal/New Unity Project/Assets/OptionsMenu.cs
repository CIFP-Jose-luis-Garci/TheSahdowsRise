using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Text fullScreenText;
    [SerializeField] TextMeshProUGUI resolution;
    [SerializeField] Slider volume;
    [SerializeField] Text volumeText;
    private bool fullScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Resolution();
        Volume();
    }
    public void FullScreenChange()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if(fullScreenText.text == "Activa")
        {
            fullScreenText.text = "Desactiva";
            fullScreen = true;
        }
        else
        {
            fullScreenText.text = "Activa";
            fullScreen = false;
        }
    }
    public void Volume()
    {
        volumeText.text = volume.value.ToString();
    }
    public void Resolution()
    {
        if (resolution.text == "1920x1080")
        {
            Screen.SetResolution(1920, 1080, fullScreen);
        }
        else if (resolution.text == "1080x720")
        {
            Screen.SetResolution(1080, 720, fullScreen);
        }
        else if (resolution.text == "800x600")
        {
            Screen.SetResolution(800, 600, fullScreen);
        }
        else if (resolution.text == "640x480")
        {
            Screen.SetResolution(640, 480, fullScreen);
        }
    }
}
