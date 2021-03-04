using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 Room0Pos;
    private Vector3 Room1Pos;
    // Start is called before the first frame update
    void Start()
    {
        Room0Pos = new Vector3(1.5f, 5.84f, -5f);
        Room1Pos = new Vector3(18.8f, 5.84f, -5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScript.currentRoom == 0)
        {
            transform.position = Room0Pos;
        }
        else if (PlayerScript.currentRoom == 1)
        {
            transform.position = Room1Pos;
        }
    }
}
