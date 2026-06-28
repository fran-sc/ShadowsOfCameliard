using UnityEngine;

// -----------------------------------------------------------------------------
// MouseFollow
//
// Responsabilidades:
// - Orienta el transform del objeto hacia la posición del cursor en el mundo.
//
// Notas:
// - Convierte la posición de pantalla del cursor a coordenadas del mundo con
//   Camera.main.ScreenToWorldPoint antes de calcular la dirección.
// -----------------------------------------------------------------------------
public class MouseFollow : MonoBehaviour
{
    void Update()
    {
        FaceMouse();
    }

    // -----------------------------------------------------------------------------
    // FaceMouse
    //
    // - Convierte la posición del ratón a coordenadas del mundo y rota el objeto
    //   para que su eje derecho apunte hacia el cursor.
    // -----------------------------------------------------------------------------
    void FaceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);


        Vector2 direction = mousePosition - transform.position;

        
        transform.right = direction.normalized; // Rotate the object to face the mouse
    }
}


