using UnityEngine;

// -----------------------------------------------------------------------------
// Destructible
//
// Responsabilidades:
// - Destruye el objeto al recibir impacto de una DamageSource.
// - Instancia un VFX de destrucción antes de eliminarse.
//
// Notas:
// - El retraso de 0.2 s en Invoke permite que la animación de impacto se inicie.
// -----------------------------------------------------------------------------
public class Destructible : MonoBehaviour
{
    [SerializeField] GameObject deathVFX;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<DamageSource>())
        {
            Invoke("Destruction", 0.2f);
        }
    }

    void Destruction()
    {
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
