using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResolution : MonoBehaviour
{
    public Dropdown dropdown;

    public Toggle toggle;

    Resolution[] res;
    void Start()
    {
        Screen.fullScreen = true;

        toggle.isOn = false;

        Resolution[] resolutions = Screen.resolutions;
        res = resolutions.Distinct().ToArray();
        string[] strRes = new string[res.Length];
        for (int i = 0; i < res.Length; i++)
        {
            strRes[i] = res[res.Length - i - 1].ToString();
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(strRes.ToList());
        dropdown.value = res.Length - 1;

        Screen.SetResolution(res[res.Length - 1].width, res[res.Length - 1].height, Screen.fullScreen);
    }
    public void SetRes()
    {
        Screen.SetResolution(res[dropdown.value].width, res[dropdown.value].height, Screen.fullScreen);
    }

    public void ScreenMode()
    {
        Screen.fullScreen = toggle.isOn;
    }
}
