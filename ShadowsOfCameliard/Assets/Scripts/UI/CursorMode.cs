using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// CursorModeController
//
// Responsabilidades:
// - Oculta el cursor al hacer clic y lo muestra al pulsar Escape.
//
// Atributos principales:
// - mouseIcon: textura del cursor personalizado.
// - sizeFactor: factor para calcular el hotspot del cursor.
// -----------------------------------------------------------------------------
public class CursorModeController : MonoBehaviour
{    
    [Header("Cursor Icon Settings")]
    [SerializeField] Texture2D mouseIcon;
    [SerializeField] Vector2 hotSpot = Vector2.zero;  

    void Awake()
    {
        Cursor.SetCursor(
            mouseIcon, 
            hotSpot,
            CursorMode.Auto);        
    }

    void Start()
    {
        //HideCursor(!MenuManager.Instance.IsMenuOpen);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {            
            HideCursor(false);
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (MenuManager.Instance.IsMenuOpen)
            {
                // Si hay un menú activo, no ocultamos el cursor
                return;
            }

            HideCursor(true);
        }
    }

    public void HideCursor(bool hideCursor)
    {
        Cursor.lockState = hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hideCursor;
    }
}
