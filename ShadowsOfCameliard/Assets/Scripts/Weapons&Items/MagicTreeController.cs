using UnityEngine;

// -----------------------------------------------------------------------------
// MagicTreeController
//
// Responsabilidades:
// - Activa la animación de crecimiento del árbol mágico al ser invocado.
// - Desactiva el collider sólido para que el jugador pueda atravesar el árbol.
//
// Notas:
// - El Animator empieza desactivado; se activa al llamar a ActivateTree().
// - Es activado por Eagle cuando el jugador interactúa con ella.
// -----------------------------------------------------------------------------
public class MagicTreeController : MonoBehaviour
{
    [SerializeField] Collider2D solidCollider;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.enabled = false;
    }

    public void ActivateTree()
    {
        // Activamos la animación de crecimiento del árbol
        anim.enabled = true;

        // Desactivamos el collider sólido para que el jugador pueda pasar
        solidCollider.enabled = false;
    }
}
