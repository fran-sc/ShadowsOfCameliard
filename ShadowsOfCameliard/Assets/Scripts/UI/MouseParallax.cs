using UnityEngine;
using UnityEngine.InputSystem;

// Aplica un efecto de parallax basado en la posición del ratón,
// desplazando el objeto de forma suave respecto a su posición inicial.
public class MouseParallax : MonoBehaviour
{
    // Multiplicador que controla cuánto se desplaza el objeto con el ratón.
    [SerializeField] float parallaxIntensity = 1f;

    // Tiempo de suavizado usado por SmoothDamp para evitar movimientos bruscos.
    [SerializeField] float smoothTime = .5f;
   
    // Posición base del objeto al iniciar la escena.
    Vector3 startPosition;

    // Velocidad interna requerida por SmoothDamp (se actualiza automáticamente).
    Vector3 velocity;

    void Start()
    {
        // Guarda la posición inicial para calcular desplazamientos relativos.
        startPosition = transform.position;
    }

    void Update()
    {
        // Actualiza el efecto en cada frame.
        ParallaxEffect();
    }

    void ParallaxEffect()
    {
        // Convierte la posición del ratón de píxeles de pantalla a coordenadas de viewport [0,1].
        Vector2 offset = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        
        // Calcula la posición objetivo sumando el offset escalado a la posición inicial.
        Vector3 targetPosition = startPosition + new Vector3(offset.x, offset.y, 0) * parallaxIntensity;

        // Interpola suavemente desde la posición actual hacia la posición objetivo.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
