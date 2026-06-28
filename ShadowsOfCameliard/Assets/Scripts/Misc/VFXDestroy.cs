using UnityEngine;

// -----------------------------------------------------------------------------
// VFXDestroy
//
// Responsabilidades:
// - Destruye automáticamente el GameObject cuando el sistema de partículas termina.
//
// Notas:
// - ps.IsAlive() comprueba el sistema raíz y sus subsistemas hijos.
// -----------------------------------------------------------------------------
public class VFXDestroy : MonoBehaviour
{
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(ps && !ps.IsAlive())
        {
            Destroy(gameObject);
        }        
    }
}
