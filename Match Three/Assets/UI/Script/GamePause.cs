using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public static bool GameIsPause = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }    
    void Resume()
    {
        Time.timeScale = 1f;
        GameIsPause = false;
    }
    
    void Pause()
    {        
        Time.timeScale = 0f;
        GameIsPause = true;
    }
}
