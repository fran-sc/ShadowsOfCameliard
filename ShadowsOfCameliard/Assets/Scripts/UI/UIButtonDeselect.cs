using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonDeselect : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Botón clicado
        

        EventSystem.current.SetSelectedGameObject(null);
    }
}
