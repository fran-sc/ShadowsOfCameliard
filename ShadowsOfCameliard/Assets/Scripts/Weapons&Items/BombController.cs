using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// BombController
//
// Responsabilidades:
// - Controla el ciclo de vida de la bomba: parpadeo, explosión y destrucción.
// - Activa el collider de explosión y el sonido en el momento de detonar.
// - Se desplaza ligeramente al instanciarse para no dañar al jugador.
//
// Atributos principales:
// - delayBeforeExplosion: tiempo de cuenta atrás antes de explotar.
// - explosionTime: tiempo que el collider de explosión permanece activo.
// - blinkInterval: frecuencia del parpadeo de aviso previo a la explosión.
//
// Notas:
// - El parpadeo usa un toggle del SpriteRenderer para crear el efecto visual.
// - HasExploded es consultado por ExplosionDamageSource para validar el daño.
// -----------------------------------------------------------------------------
public class BombController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] float delayBeforeExplosion = 3f;
    [SerializeField] float explosionTime = 0.8f;
    [SerializeField] CircleCollider2D explosionCollider;
    [SerializeField] float blinkInterval = 0.1f;

    Animator anim;
    SpriteRenderer sr;

    bool hasExploded = false;
    public bool HasExploded => hasExploded;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        explosionCollider.enabled = false;
        
        // Desplazmos el objeto hacia adelante para evitar que el jugador se dañe a sí mismo al colocar la bomba
        transform.position += new Vector3(transform.localScale.x * 0.5f, 0, 0);

        StartCoroutine(ExplosionTimer());
    }

    // -----------------------------------------------------------------------------
    // ExplosionTimer
    //
    // - Hace parpadear el sprite durante delayBeforeExplosion segundos.
    // - Activa el collider de explosión y el sonido al terminar la cuenta atrás.
    // - Destruye el GameObject tras explosionTime segundos.
    // -----------------------------------------------------------------------------
    IEnumerator ExplosionTimer()
    {
        float t = 0;

        // Parpadeo del sprite antes de la explosión
        while (t < delayBeforeExplosion)
        {
            sr.enabled = !sr.enabled; // Alternamos la visibilidad del sprite para crear un efecto de parpadeo
            yield return new WaitForSeconds(blinkInterval);
            t += blinkInterval;
        }
        sr.enabled = true; // Aseguramos que el sprite esté visible al explotar

        hasExploded = true;

        // Reproducimos el sonido de la explosión
        AudioManager.Instance.PlayEffect(AudioManager.Effect.Explosion);

        // Activamos el animator para reproducir la animación de explosión
        anim.enabled = true; 
        explosionCollider.enabled = true; // Activamos el collider de la explosión

        yield return new WaitForSeconds(explosionTime);

        Destroy(gameObject); // Destruye el objeto después de la explosión
    }     
}
