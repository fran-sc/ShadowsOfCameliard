# GameManager

`GameManager` es un manager global deliberadamente ligero. En la versión actual de la demo, su única responsabilidad práctica es establecer el clip de música de fondo que debe reproducirse durante la partida.

El proyecto no concentra la lógica principal del juego en `GameManager`. En su lugar, la arquitectura se basa en múltiples subsistemas con responsabilidades claras y bastante independientes.

## Responsabilidad actual

```csharp
public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] AudioClip mainTheme;

    void Start()
    {
        AudioManager.Instance.PlayMusic(mainTheme);
    }
}
```

Flujo:

1. Se inicia la escena jugable.
2. `GameManager` toma el clip `mainTheme` asignado en Inspector.
3. Solicita a `AudioManager` que lo reproduzca.
4. El resto de sistemas funciona de forma independiente.

## Decisión de diseño

Mantener `GameManager` pequeño evita convertirlo en una clase difícil de mantener. Las responsabilidades están repartidas:

- `AudioManager`: reproducción de audio.
- `InputManager`: entrada del jugador.
- `InventoryManager`: inventario.
- `ResourcesManager`: recursos persistentes.
- `SceneManagement`: carga de recursos y portales.
- `MessageManager`: mensajes.
- `UIManager`: HUD.

## Posible evolución futura

En una versión posterior, `GameManager` podría coordinar estados globales de partida: pausa, victoria, derrota, modo cinemática o progreso general. En la demo actual no asume esas funciones.
