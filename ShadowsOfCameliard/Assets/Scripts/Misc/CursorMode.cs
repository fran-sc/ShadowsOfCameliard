using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// CursorModeController
//
// Responsabilidades:
// - Configura el cursor personalizado al iniciar el juego.
// - Oculta el cursor al hacer clic y lo muestra al pulsar Escape.
//
// Atributos principales:
// - mouseIcon: textura del cursor personalizado.
// - sizeFactor: factor para calcular el hotspot del cursor.
// -----------------------------------------------------------------------------
public class CursorModeController : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] Texture2D mouseIcon;
    [SerializeField] Vector2 sizeFactor = Vector2.zero;

    void Start()
    {   
        Cursor.SetCursor(
            mouseIcon, 
            new Vector2(mouseIcon.width * sizeFactor.x, mouseIcon.height * sizeFactor.y),
            CursorMode.Auto);

        SetCursorState(true);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorState(false);
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetCursorState(true);
        }
    }

    void SetCursorState(bool hideCursor)
    {
        Cursor.lockState = hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hideCursor;
    }
}
