using System.Collections;
using NUnit.Framework;
using UnityEngine;

// -----------------------------------------------------------------------------
// PushBack
//
// Responsabilidades:
// - Aplica un impulso puntual al Rigidbody2D al recibir un golpe.
// - Gestiona el flag IsPushed para que otros sistemas bloqueen su lógica.
// - Detiene el empuje automáticamente tras pushTime segundos.
//
// Atributos principales:
// - pushTime: duración del efecto de empuje en segundos.
//
// Notas:
// - Cancela velocidades previas antes de aplicar el impulso para consistencia.
// - Si se recibe un nuevo empuje mientras otro está activo, el anterior se cancela.
// -----------------------------------------------------------------------------
public class PushBack : MonoBehaviour
{
    [SerializeField] float pushTime = 0.25f;

    Rigidbody2D rb;
    Coroutine stopPushCoroutine;

    public bool IsPushed {get; private set;} = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // -----------------------------------------------------------------------------
    // Push
    //
    // - Cancela la velocidad actual, activa IsPushed y aplica un impulso de dirección
    //   × fuerza × masa para que sea independiente del peso del objeto.
    // - Lanza StopPush para restaurar el estado tras pushTime segundos.
    // -----------------------------------------------------------------------------
    public void Push(Vector2 direction, float force)
    {
        if (rb == null)
        {
            Debug.LogWarning("PushBack: Rigidbody2D no encontrado.");
            return;
        }

        if (stopPushCoroutine != null)
        {
            StopCoroutine(stopPushCoroutine);
            stopPushCoroutine = null;
        }

        IsPushed = true;

        // Cancelamos valocidad anterior para que el empuje sea consistente
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.AddForce(direction * force * rb.mass, ForceMode2D.Impulse);
        
        stopPushCoroutine = StartCoroutine(StopPush());
    }

    IEnumerator StopPush()
    {
        yield return new WaitForSeconds(pushTime);
        
        StopPushInmediately();
    }

    void StopPushInmediately()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        IsPushed = false;
        stopPushCoroutine = null;
    }

    void OnDisable()
    {
        if (stopPushCoroutine != null)
        {
            StopCoroutine(stopPushCoroutine);
            stopPushCoroutine = null;
        }

        IsPushed = false;
    }
}
