using UnityEngine;

// -----------------------------------------------------------------------------
// MessageTest
//
// Clase de prueba para verificar el funcionamiento de MessageManager.
// No forma parte del flujo de juego final.
// -----------------------------------------------------------------------------
public class MessageTest : MonoBehaviour
{
    void Start()
    {
        MessageManager.Instance.ShowMessage("¡Has recogido un objeto!");
    }

    void Update()
    {
        
    }
}
