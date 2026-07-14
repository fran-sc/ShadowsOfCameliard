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
    void Start()
    {   
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
