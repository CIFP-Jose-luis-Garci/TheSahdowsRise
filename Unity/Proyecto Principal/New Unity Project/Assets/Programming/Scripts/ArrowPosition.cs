using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPosition : MonoBehaviour
{
    [SerializeField] Transform newGame;
    [SerializeField] Transform contGame;
    [SerializeField] Transform options;
    [SerializeField] Transform credits;
    [SerializeField] Transform exit;
    public void NewGame()
    {
        transform.position = newGame.position;
    }
    public void ContinueGame()
    {
        transform.position = contGame.position;
    }
    public void Options()
    {
        transform.position = options.position;
    }
    public void Credits()
    {
        transform.position = credits.position;
    }
    public void Exit()
    {
        transform.position = exit.position;
    }
}
