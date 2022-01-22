using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Forces : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Power;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Power.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Power.SetActive(false);
    }
}
