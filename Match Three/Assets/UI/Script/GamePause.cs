using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public static bool GameIsPause = false;

    // Update is called once per frame
    public void ChangeState()
    {
        GameIsPause = !GameIsPause;
    }    
}
