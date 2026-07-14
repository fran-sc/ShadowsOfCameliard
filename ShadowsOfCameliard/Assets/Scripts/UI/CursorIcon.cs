using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    [Header("Cursor Icon Settings")]
    [SerializeField] Texture2D mouseIcon;
    [SerializeField] Vector2 hotSpot = Vector2.zero;  


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Cursor.SetCursor(
            mouseIcon, 
            hotSpot,
            CursorMode.Auto);        
    }
}
