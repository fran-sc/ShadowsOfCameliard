using UnityEngine;

// -----------------------------------------------------------------------------
// SpawnedItemCollect
//
// Responsabilidades:
// - Detecta la colisión del jugador con el item y notifica al sistema de recogida.
// - Reproduce el sonido de recogida y destruye el GameObject.
//
// Notas:
// - Usa PlayerCollectSubject para notificar a todos los observadores registrados.
// - El nombre del GameObject se usa como itemID para identificar el recurso.
// -----------------------------------------------------------------------------
public class SpawnedItemCollect : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCollectSubject>()?.NotifyObservers(gameObject.name);

            // Reproducimos el sonido de recogida de objeto
            AudioManager.Instance.PlayEffect(AudioManager.Effect.ItemCollect);

            // Después de notificar a los observadores, destruimos el objeto
            Destroy(gameObject);
        }
    }
}
