using UnityEngine;

// -----------------------------------------------------------------------------
// EnemyDetector
//
// Responsabilidades:
// - Detecta la entrada, permanencia y salida del jugador en el rango de detección.
// - Notifica a EnemyAI para iniciar o detener la persecución.
//
// Notas:
// - OnTriggerStay2D garantiza la reanudación de la persecución si EnemyAI se
//   reactiva con el jugador ya dentro del área.
// - Al desactivarse limpia la referencia al jugador para evitar referencias colgantes.
// -----------------------------------------------------------------------------
public class EnemyDetector : MonoBehaviour
{
    [SerializeField] Collider2D detector;

    EnemyAI enemyAI;    
    GameObject player;

    void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyAI == null || !enemyAI.isActiveAndEnabled)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;

            enemyAI.StartChasing(player);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (enemyAI == null || !enemyAI.isActiveAndEnabled)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;

            enemyAI.StartChasing(player);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (enemyAI == null || !enemyAI.isActiveAndEnabled)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            player = null;

            enemyAI.StopChasing();
        }
    }

    void OnDisable()
    {
        player = null;
    }
}
