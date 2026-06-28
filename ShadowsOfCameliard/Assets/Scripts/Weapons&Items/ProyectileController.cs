using UnityEngine;

// -----------------------------------------------------------------------------
// ProyectileController
//
// Responsabilidades:
// - Mueve el proyectil hacia adelante a velocidad constante.
// - Destruye el proyectil tras superar su tiempo de vida máximo.
//
// Atributos principales:
// - speed: velocidad de desplazamiento.
// - lifeTime: tiempo máximo en segundos antes de autodestruirse.
//
// Notas:
// - La dirección de vuelo se deriva de la escala local X del proyectil,
//   que hereda la orientación del jugador en el momento del disparo.
// -----------------------------------------------------------------------------
public class ProyectileController : MonoBehaviour
{
    [Header("Proyectile Settings")]
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 5f;

    Rigidbody2D rb;
    float timer;
    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        timer = 0f;

        // La dirección se basa en la escala local del proyectil (mirando hacia donde apunta el arma)
        direction = new Vector2(transform.localScale.x, 0).normalized; 
    }

    void FixedUpdate()
    {
        // Movemos el proyectil hacia adelante
        rb.MovePosition(rb.position + (direction * speed * Time.fixedDeltaTime));
        
        // Destruimos el proyectil después de su tiempo de vida máxima
        timer += Time.fixedDeltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
