using UnityEngine;

// -----------------------------------------------------------------------------
// BlockadeDestroy
//
// Responsabilidades:
// - Gestiona la destrucción visual de un bloqueo al recibir una explosión.
// - Cambia el sprite al destruido, desactiva el collider y actualiza el estado del recurso.
//
// Notas:
// - El estado Destroyed evita que la blockade vuelva a aparecer al recargar la escena.
// - El nombre del GameObject se usa como itemID para acceder al recurso.
// -----------------------------------------------------------------------------
public class BlockadeDestroy : MonoBehaviour
{
    [SerializeField] Sprite destroyedSprite;
    SpriteRenderer sr;
    
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void DestroyBlockade()
    {
        // Cambiamos el sprite del objeto a la versión destruida
        sr.sprite = destroyedSprite;

        // Desactivamos el collider para que el jugador pueda pasar
        GetComponent<Collider2D>().enabled = false; 

        // Cambiamos el estado del recurso en el ResourcesManager para que no 
        // vuelva a aparecer al cargar la escena
        SpawnOnceSO resource = ResourcesManager.Instance.GetResource(gameObject.name);
        if (resource != null)
        {
            resource.state = SpawnOnceSO.SpawnOnceState.Destroyed;
        }
    }
}
