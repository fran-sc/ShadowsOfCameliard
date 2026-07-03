using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTER: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXIT: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK: " + gameObject.name);
    }
}