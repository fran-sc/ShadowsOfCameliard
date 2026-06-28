# Introducción del proyecto

**Sombras de Cameliard** es una demo jugable 2D top-down de acción/RPG ambientada en una reinterpretación artúrica. Lancelot atraviesa las tierras invadidas de Cameliard para rescatar a la reina Ginebra, cautiva tras la ofensiva de Mordred.

La demo combina exploración, combate básico, recogida de armas, transición entre escenas, recursos persistentes y una presentación narrativa mediante códices ilustrados.

## Datos técnicos

| Campo | Valor |
|---|---|
| Motor | Unity |
| Versión | Unity 6000.3.9f1 |
| Lenguaje | C# |
| Render pipeline | URP |
| Género | 2D top-down, RPG simple, acción, arcade |
| Estilo visual | Pixel art |
| Plataformas objetivo | Windows, WebGL, Linux |

## Objetivo de la demo

El objetivo es tener una demo pequeña pero completa, con un bucle funcional:

1. Introducción narrativa en formato códice.
2. Inicio en la campiña de Cameliard.
3. Recogida de la espada inicial, `Steel of Benwick`.
4. Uso del inventario y equipamiento.
5. Exploración del mapa exterior.
6. Encuentros con enemigos.
7. Desbloqueos mediante personajes y objetos.
8. Entrada a la cueva.
9. Recogida del arco, `Star of Avalon`
10. Desbloqueo del paso de la `Campa de los Druidas`.
10. Recogida de la pócima explosiva, `Flame of Pendragon`
11. Regreso o avance hacia el final de la demo.
12. Desbloqueo de los puentes de acceso a la ciudad de Cameliard.
13. Cierre narrativo en el códice.

## Enfoque técnico

La arquitectura favorece sistemas simples, separados y mantenibles. El proyecto no usa un único controlador central que gobierne toda la partida. En su lugar, reparte responsabilidades en managers especializados:

- `GameManager`: actualmente sólo establece la música principal.
- `AudioManager`: reproduce música y efectos.
- `InputManager`: centraliza el input.
- `InventoryManager`: gestiona inventario y equipamiento.
- `ResourcesManager`: carga de recursos.
- `SceneManagement`: gestiona recursos de escena y puntos de aparición.
- `MessageManager`: muestra mensajes contextuales.
- `UIManager`: actualiza el HUD.

[< volver](README.md)